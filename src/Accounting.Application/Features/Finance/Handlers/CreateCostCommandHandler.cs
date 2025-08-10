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
    public class CreateCostCommandHandler : ICommandHandler<CreateCostCommand, Result<CostDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IDocumentNumberService _documentNumberService;

        public CreateCostCommandHandler(IAccountingDbContext context, IDocumentNumberService documentNumberService)
        {
            _context = context;
            _documentNumberService = documentNumberService;
        }

        public async Task<Result<CostDto>> Handle(CreateCostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Custom validation
                if (!request.IsValid(out string validationError))
                {
                    return Result.Failure<CostDto>(validationError);
                }

                // Validate bank account if payment source is bank
                BankAccount? bankAccount = null;
                if (request.PaymentSource == PaymentSource.Bank)
                {
                    bankAccount = await _context.BankAccounts
                        .FirstOrDefaultAsync(ba => ba.Id == request.BankAccountId, cancellationToken);
                    
                    if (bankAccount == null)
                    {
                        return Result.Failure<CostDto>("Bank account not found");
                    }

                    // Check if bank account has sufficient balance
                    var currentBalance = await GetAccountBalance(request.BankAccountId.Value, cancellationToken);
                    if (currentBalance < request.Amount)
                    {
                        return Result.Failure<CostDto>($"Insufficient balance in bank account. Available: {currentBalance:C}");
                    }
                }

                // Validate counterparty if provided
                Counterparty? counterparty = null;
                if (request.CounterpartyId.HasValue)
                {
                    counterparty = await _context.Counterparties
                        .FirstOrDefaultAsync(c => c.Id == request.CounterpartyId, cancellationToken);
                    
                    if (counterparty == null)
                    {
                        return Result.Failure<CostDto>("Counterparty not found");
                    }
                }

                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync(
                    "COST", 
                    request.Company, 
                    cancellationToken);

                // Calculate local amount
                var localAmount = request.Currency == "IRR" ? request.Amount : request.Amount * (request.ExchangeRate ?? 1);

                var cost = new Cost
                {
                    DocumentNumber = documentNumber,
                    Date = request.Date,
                    Description = request.Description,
                    Amount = request.Amount,
                    LocalAmount = localAmount,
                    Currency = request.Currency,
                    ExchangeRate = request.ExchangeRate ?? 1,
                    PaymentSource = request.PaymentSource,
                    BankAccountId = request.BankAccountId,
                    CounterpartyId = request.CounterpartyId,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    Status = CostStatus.Posted,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Costs.Add(cost);
                await _context.SaveChangesAsync(cancellationToken);

                // Create ledger entry if payment source is bank
                if (request.PaymentSource == PaymentSource.Bank && bankAccount != null)
                {
                    await CreateLedgerEntryAsync(cost, bankAccount, cancellationToken);
                }

                var costDto = new CostDto
                {
                    Id = cost.Id,
                    DocumentNumber = cost.DocumentNumber,
                    Date = cost.Date,
                    Description = cost.Description,
                    Amount = cost.Amount,
                    LocalAmount = cost.LocalAmount,
                    Currency = cost.Currency,
                    ExchangeRate = cost.ExchangeRate,
                    PaymentSource = cost.PaymentSource,
                    BankAccountId = cost.BankAccountId,
                    BankAccountName = bankAccount?.AccountName,
                    CounterpartyId = cost.CounterpartyId,
                    CounterpartyName = counterparty?.Name,
                    Reference = cost.Reference,
                    Notes = cost.Notes,
                    Status = cost.Status,
                    Company = cost.Company,
                    CreatedAt = cost.CreatedAt,
                    UpdatedAt = cost.UpdatedAt
                };

                return Result<CostDto>.Success(costDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<CostDto>($"Error creating cost: {ex.Message}");
            }
        }

        private async Task<decimal> GetAccountBalance(int accountId, CancellationToken cancellationToken)
        {
            var balance = await _context.LedgerEntries
                .Where(le => le.AccountCode == "BANK")
                .SumAsync(le => le.LocalDebitAmount - le.LocalCreditAmount, cancellationToken);

            return balance;
        }

        private async Task CreateLedgerEntryAsync(Cost cost, BankAccount bankAccount, CancellationToken cancellationToken)
        {
            // Credit: Bank Account (decrease balance for cost)
            var ledgerEntry = new LedgerEntry
            {
                Date = cost.Date,
                DocumentNumber = cost.DocumentNumber,
                DocumentType = "COST",
                DocumentId = cost.Id,
                Description = cost.Description,
                AccountCode = "BANK", // Using generic bank account code since BankAccount doesn't have AccountCode
                AccountName = bankAccount.AccountName,
                DebitAmount = 0,
                CreditAmount = cost.Amount,
                Currency = cost.Currency,
                ExchangeRate = cost.ExchangeRate,
                LocalDebitAmount = 0,
                LocalCreditAmount = cost.LocalAmount,
                CounterpartyId = cost.CounterpartyId,
                Reference = cost.Reference,
                Company = cost.Company,
                CreatedBy = cost.CreatedBy
            };

            _context.LedgerEntries.Add(ledgerEntry);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}