using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;
using Accounting.Application.Common.Models;
using Accounting.Domain.Enums;
using Accounting.Domain.Entities;
using AutoMapper;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class UpdateVoucherCommandHandler : ICommandHandler<UpdateVoucherCommand, Result<VoucherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateVoucherCommandHandler> _logger;

        public UpdateVoucherCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateVoucherCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers
                    .GetByIdWithIncludesAsync(request.Id, v => v.Entries);

                if (voucher == null)
                {
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                // Check if voucher can be updated
                if (voucher.Status == VoucherStatus.Posted)
                {
                    return Result.Failure<VoucherDto>("Cannot update a posted voucher");
                }

                if (voucher.Status == VoucherStatus.Approved && voucher.IsPosted)
                {
                    return Result.Failure<VoucherDto>("Cannot update an approved and posted voucher");
                }

                // Validate entries if provided
                if (request.Entries != null && request.Entries.Any())
                {
                    // Validate balance
                    var totalDebit = request.Entries.Sum(e => e.DebitAmount);
                    var totalCredit = request.Entries.Sum(e => e.CreditAmount);
                    
                    if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                    {
                        return Result.Failure<VoucherDto>("Voucher entries must be balanced (total debits must equal total credits)");
                    }

                    // Validate accounts exist
                    var accountIds = request.Entries.Select(e => e.AccountId).Distinct().ToList();
                    var accounts = await _unitOfWork.Accounts.FindAsync(a => accountIds.Contains(a.Id));
                    if (accounts.Count() != accountIds.Count)
                    {
                        return Result.Failure<VoucherDto>("One or more accounts not found");
                    }

                    // Update amount
                    voucher.Amount = totalDebit;
                }

                // Update voucher properties
                voucher.Description = request.Description ?? voucher.Description;
                voucher.VoucherDate = request.VoucherDate;
                voucher.Reference = request.Reference ?? voucher.Reference;
                voucher.Notes = request.Notes ?? voucher.Notes;

                // Update entries if provided
                if (request.Entries != null)
                {
                    // Remove existing entries
                    voucher.Entries.Clear();

                    // Add new entries
                    foreach (var entryDto in request.Entries)
                    {
                        var entry = new VoucherEntry
                        {
                            Description = entryDto.Description,
                            Amount = entryDto.DebitAmount > 0 ? entryDto.DebitAmount : entryDto.CreditAmount,
                            TransactionType = entryDto.DebitAmount > 0 ? TransactionType.Debit : TransactionType.Credit,
                            Currency = entryDto.Currency ?? voucher.Currency,
                            VoucherId = voucher.Id,
                            AccountId = entryDto.AccountId
                        };
                        
                        voucher.Entries.Add(entry);
                    }
                }

                await _unitOfWork.Vouchers.UpdateAsync(voucher);
                await _unitOfWork.SaveChangesAsync();

                var voucherDto = _mapper.Map<VoucherDto>(voucher);
                
                _logger.LogInformation("Voucher {VoucherNumber} updated successfully", voucher.VoucherNumber);
                
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>("An error occurred while updating the voucher");
            }
        }
    }
}