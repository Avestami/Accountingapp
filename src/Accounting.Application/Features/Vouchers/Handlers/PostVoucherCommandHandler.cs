using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Application.Common;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class PostVoucherCommandHandler : ICommandHandler<PostVoucherCommand, Result<VoucherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PostVoucherCommandHandler> _logger;

        public PostVoucherCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PostVoucherCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(PostVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers
                    .GetByIdWithIncludesAsync(request.Id, v => v.Entries, v => v.Entries.Select(e => e.Account));

                if (voucher == null)
                {
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                // Check if voucher can be posted
                if (voucher.Status != VoucherStatus.Approved)
                {
                    return Result.Failure<VoucherDto>("Only approved vouchers can be posted");
                }

                if (voucher.IsPosted)
                {
                    return Result.Failure<VoucherDto>("Voucher is already posted");
                }

                // Validate voucher is balanced
                var totalDebit = voucher.Entries.Where(e => e.TransactionType == TransactionType.Debit).Sum(e => e.Amount);
                    var totalCredit = voucher.Entries.Where(e => e.TransactionType == TransactionType.Credit).Sum(e => e.Amount);
                
                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    return Result.Failure<VoucherDto>("Cannot post unbalanced voucher");
                }

                // Validate all entries have valid accounts
                if (voucher.Entries.Any(e => e.Account == null))
                {
                    return Result.Failure<VoucherDto>("All entries must have valid accounts");
                }

                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();
                
                try
                {
                    // Post the voucher
                    voucher.Post(request.PostedByUserId);
                    voucher.Status = VoucherStatus.Posted;
                    
                    if (!string.IsNullOrEmpty(request.Notes))
                    {
                        voucher.Notes = string.IsNullOrEmpty(voucher.Notes) 
                            ? request.Notes 
                            : $"{voucher.Notes}\n\nPosting Notes: {request.Notes}";
                    }

                    // Update account balances
                    foreach (var entry in voucher.Entries)
                    {
                        var account = entry.Account;
                        
                        if (entry.TransactionType == TransactionType.Debit && entry.Amount > 0)
                        {
                            // Increase debit balance or decrease credit balance
                            if (account.Type == AccountType.Asset || account.Type == AccountType.Expense)
                            {
                                account.Balance += entry.Amount;
                            }
                            else
                            {
                                account.Balance -= entry.Amount;
                            }
                        }
                        
                        if (entry.TransactionType == TransactionType.Credit && entry.Amount > 0)
                        {
                            // Increase credit balance or decrease debit balance
                            if (account.Type == AccountType.Liability || 
                                account.Type == AccountType.Equity || 
                                account.Type == AccountType.Revenue)
                            {
                                account.Balance += entry.Amount;
                            }
                            else
                            {
                                account.Balance -= entry.Amount;
                            }
                        }
                        
                        await _unitOfWork.Accounts.UpdateAsync(account);
                    }
                    
                    await _unitOfWork.Vouchers.UpdateAsync(voucher);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    var voucherDto = _mapper.Map<VoucherDto>(voucher);
                    
                    _logger.LogInformation("Voucher {VoucherNumber} posted successfully", voucher.VoucherNumber);
                    
                    return Result.Success<VoucherDto>(voucherDto);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>("An error occurred while posting the voucher");
            }
        }
    }
}