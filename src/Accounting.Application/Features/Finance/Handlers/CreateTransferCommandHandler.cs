using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Application.Services;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class CreateTransferCommandHandler : ICommandHandler<CreateTransferCommand, Result<TransferDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IDocumentNumberService _documentNumberService;

        public CreateTransferCommandHandler(IAccountingDbContext context, IDocumentNumberService documentNumberService)
        {
            _context = context;
            _documentNumberService = documentNumberService;
        }

        public async Task<Result<TransferDto>> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Custom validation
                if (!request.IsValid(out string validationError))
                {
                    return Result<TransferDto>.Failure(validationError);
                }

                // Validate bank accounts exist and belong to the company
                var fromBankAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(ba => ba.Id == request.FromAccountId && ba.Company == request.Company, cancellationToken);
                
                var toBankAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(ba => ba.Id == request.ToAccountId && ba.Company == request.Company, cancellationToken);

                if (fromBankAccount == null)
                {
                    return Result<TransferDto>.Failure("From bank account not found");
                }

                if (toBankAccount == null)
                {
                    return Result<TransferDto>.Failure("To bank account not found");
                }

                // Check if from account has sufficient balance
                var currentBalance = await GetAccountBalance(request.FromAccountId, request.Company, cancellationToken);
                if (currentBalance < request.Amount)
                {
                    return Result<TransferDto>.Failure($"Insufficient balance in from account. Available: {currentBalance:C}");
                }

                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync(
                    DocumentType.Transfer, 
                    request.Company, 
                    cancellationToken);

                // Calculate local amount
                var localAmount = request.Currency == "IRR" ? request.Amount : request.Amount * (request.ExchangeRate ?? 1);

                var transfer = new Transfer
                {
                    DocumentNumber = documentNumber,
                    Date = request.Date,
                    Description = request.Description,
                    Amount = request.Amount,
                    LocalAmount = localAmount,
                    Currency = request.Currency,
                    ExchangeRate = request.ExchangeRate ?? 1,
                    FromBankAccountId = request.FromAccountId,
                    ToBankAccountId = request.ToAccountId,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    Status = TransferStatus.Pending,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync(cancellationToken);

                // Create ledger entries
                await CreateLedgerEntriesAsync(transfer, fromBankAccount, toBankAccount, cancellationToken);

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
                    FromAccountName = fromBankAccount.AccountName,
                    ToAccountId = transfer.ToBankAccountId,
                    ToAccountName = toBankAccount.AccountName,
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
                return Result<TransferDto>.Failure($"Error creating transfer: {ex.Message}");
            }
        }

        private async Task<decimal> GetAccountBalance(int accountId, string company, CancellationToken cancellationToken)
        {
            var balance = await _context.LedgerEntries
                .Where(le => le.BankAccountId == accountId && le.Company == company)
                .SumAsync(le => le.LocalDebitAmount - le.LocalCreditAmount, cancellationToken);

            return balance;
        }

        private async Task CreateLedgerEntriesAsync(Transfer transfer, BankAccount fromAccount, BankAccount toAccount, CancellationToken cancellationToken)
        {
            // Debit: From Account (decrease balance)
            var fromEntry = new LedgerEntry
            {
                Date = transfer.Date,
                DocumentNumber = transfer.DocumentNumber,
                Description = $"Transfer to {toAccount.AccountName}",
                DebitAmount = 0,
                CreditAmount = transfer.Amount,
                Currency = transfer.Currency,
                ExchangeRate = transfer.ExchangeRate,
                LocalDebitAmount = 0,
                LocalCreditAmount = transfer.LocalAmount,
                BankAccountId = transfer.FromBankAccountId,
                Reference = transfer.Reference,
                Company = transfer.Company
            };

            // Credit: To Account (increase balance)
            var toEntry = new LedgerEntry
            {
                Date = transfer.Date,
                DocumentNumber = transfer.DocumentNumber,
                Description = $"Transfer from {fromAccount.AccountName}",
                DebitAmount = transfer.Amount,
                CreditAmount = 0,
                Currency = transfer.Currency,
                ExchangeRate = transfer.ExchangeRate,
                LocalDebitAmount = transfer.LocalAmount,
                LocalCreditAmount = 0,
                BankAccountId = transfer.ToBankAccountId,
                Reference = transfer.Reference,
                Company = transfer.Company
            };

            _context.LedgerEntries.Add(fromEntry);
            _context.LedgerEntries.Add(toEntry);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}