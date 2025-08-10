using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Application.Services;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class UpdateTransferCommandHandler : ICommandHandler<UpdateTransferCommand, Result<TransferDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;

        public UpdateTransferCommandHandler(IAccountingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<TransferDto>> Handle(UpdateTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Custom validation
                if (!request.IsValid(out string validationError))
                {
                    return Result.Failure<TransferDto>(validationError);
                }

                var transfer = await _context.Transfers
                    .Include(t => t.FromBankAccount)
                    .Include(t => t.ToBankAccount)
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.Company == request.Company, cancellationToken);

                if (transfer == null)
                {
                    return Result.Failure<TransferDto>("Transfer not found");
                }

                // Only pending transfers can be updated
                if (transfer.Status != TransferStatus.Pending)
                {
                    return Result.Failure<TransferDto>("Only pending transfers can be updated");
                }

                // Validate bank accounts exist and belong to the company
                var fromAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(ba => ba.Id == request.FromAccountId && ba.Company == request.Company, cancellationToken);
                
                var toAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(ba => ba.Id == request.ToAccountId && ba.Company == request.Company, cancellationToken);

                if (fromAccount == null)
                {
                    return Result.Failure<TransferDto>("From bank account not found");
                }

                if (toAccount == null)
                {
                    return Result.Failure<TransferDto>("To bank account not found");
                }

                // Check if accounts have sufficient balance (for from account)
                var currentBalance = await GetAccountBalance(request.FromAccountId, request.Company, cancellationToken);
                if (currentBalance < request.Amount && request.FromAccountId != transfer.FromBankAccountId)
                {
                    return Result.Failure<TransferDto>($"Insufficient balance in from account. Available: {currentBalance:C}");
                }

                // Update transfer properties
                transfer.Date = request.Date;
                transfer.Description = request.Description;
                transfer.Amount = request.Amount;
                transfer.Currency = request.Currency;
                transfer.ExchangeRate = request.ExchangeRate ?? 1;
                transfer.FromBankAccountId = request.FromAccountId;
                transfer.ToBankAccountId = request.ToAccountId;
                transfer.Reference = request.Reference;
                transfer.Notes = request.Notes;
                transfer.UpdatedAt = DateTime.UtcNow;

                // Calculate local amount
                transfer.LocalAmount = request.Currency == "IRR" ? request.Amount : request.Amount * (request.ExchangeRate ?? 1);

                await _context.SaveChangesAsync(cancellationToken);

                // Return updated transfer DTO
                var transferDto = new TransferDto
                {
                    Id = transfer.Id,
                    DocumentNumber = transfer.DocumentNumber,
                    Date = transfer.Date,
                    Description = transfer.Description,
                    Amount = transfer.Amount,
                    LocalAmount = transfer.LocalAmount,
                    Currency = transfer.Currency,
                    ExchangeRate = transfer.ExchangeRate,
                    FromAccountId = transfer.FromBankAccountId,
                    FromAccountName = fromAccount.AccountName,
                    ToAccountId = transfer.ToBankAccountId,
                    ToAccountName = toAccount.AccountName,
                    Reference = transfer.Reference,
                    Notes = transfer.Notes,
                    Status = transfer.Status,
                    Company = transfer.Company,
                    CreatedAt = transfer.CreatedAt,
                    UpdatedAt = transfer.UpdatedAt
                };

                return Result<TransferDto>.Success(transferDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<TransferDto>($"Error updating transfer: {ex.Message}");
            }
        }

        private async Task<decimal> GetAccountBalance(int accountId, string company, CancellationToken cancellationToken)
        {
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == accountId && ba.Company == company, cancellationToken);

            return account?.OpeningBalance ?? 0;
        }
    }
}