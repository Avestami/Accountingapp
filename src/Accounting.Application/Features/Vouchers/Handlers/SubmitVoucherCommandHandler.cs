using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Models;
using Accounting.Domain.Enums;
using AutoMapper;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class SubmitVoucherCommandHandler : ICommandHandler<SubmitVoucherCommand, Result<VoucherDto>>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<SubmitVoucherCommandHandler> _logger;

        public SubmitVoucherCommandHandler(
            IVoucherRepository voucherRepository,
            IAccountRepository accountRepository,
            ILogger<SubmitVoucherCommandHandler> logger)
        {
            _voucherRepository = voucherRepository;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(SubmitVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _voucherRepository.GetVoucherWithDetailsAsync(request.Id);
                if (voucher == null)
                {
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                // Check if voucher can be submitted
                if (voucher.Status != VoucherStatus.Draft)
                {
                    return Result.Failure<VoucherDto>("Only draft vouchers can be submitted");
                }

                // Validate voucher has entries
                if (!voucher.Entries.Any())
                {
                    return Result.Failure<VoucherDto>("Voucher must have at least one entry");
                }

                // Validate voucher is balanced
                var debitTotal = voucher.Entries.Where(e => e.TransactionType == TransactionType.Debit).Sum(e => e.Amount);
                var creditTotal = voucher.Entries.Where(e => e.TransactionType == TransactionType.Credit).Sum(e => e.Amount);
                
                if (debitTotal != creditTotal)
                {
                    return Result.Failure<VoucherDto>("Voucher entries must be balanced (debits must equal credits)");
                }

                // Validate all accounts exist
                var accountIds = voucher.Entries.Select(e => e.AccountId).Distinct().ToList();
                var accounts = await _accountRepository.GetByIdsAsync(accountIds);
                
                if (accounts.Count() != accountIds.Count)
                {
                    var missingAccountIds = accountIds.Except(accounts.Select(a => a.Id)).ToList();
                    return Result.Failure<VoucherDto>($"Invalid account IDs: {string.Join(", ", missingAccountIds)}");
                }

                // Submit the voucher
                voucher.Submit();
                if (!string.IsNullOrEmpty(request.Notes))
                {
                    voucher.Notes = request.Notes;
                }

                await _voucherRepository.UpdateAsync(voucher);
                await _voucherRepository.SaveChangesAsync();

                // Map to DTO
                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    VoucherNumber = voucher.VoucherNumber,
                    Type = voucher.Type,
                    TypeName = voucher.Type.ToString(),
                    Date = voucher.VoucherDate,
                    Description = voucher.Description,
                    Reference = voucher.Reference,
                    Status = voucher.Status,
                    StatusName = voucher.Status.ToString(),
                    TotalDebit = debitTotal,
                    TotalCredit = creditTotal,
                    IsBalanced = debitTotal == creditTotal,
                    IsPosted = voucher.IsPosted,
                    PostedDate = voucher.PostedDate,
                    PostedByUserId = voucher.PostedByUserId,
                    TicketId = voucher.TicketId,
                    CreatedByUserId = voucher.CreatedByUserId,
                    CreatedAt = voucher.CreatedAt,
                    UpdatedAt = voucher.UpdatedAt ?? DateTime.UtcNow
                };

                _logger.LogInformation("Voucher {VoucherId} submitted successfully", request.Id);
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>($"Error submitting voucher: {ex.Message}");
            }
        }
    }
}