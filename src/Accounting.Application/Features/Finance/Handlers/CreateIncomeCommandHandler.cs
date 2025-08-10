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
    public class CreateIncomeCommandHandler : ICommandHandler<CreateIncomeCommand, Result<IncomeDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IDocumentNumberService _documentNumberService;

        public CreateIncomeCommandHandler(IAccountingDbContext context, IDocumentNumberService documentNumberService)
        {
            _context = context;
            _documentNumberService = documentNumberService;
        }

        public async Task<Result<IncomeDto>> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Custom validation
                if (!request.IsValid(out string validationError))
                {
                    return Result.Failure<IncomeDto>(validationError);
                }

                // Validate bank account if payment source is bank
                BankAccount? bankAccount = null;
                if (request.PaymentSource == PaymentSource.Bank)
                {
                    bankAccount = await _context.BankAccounts
                        .FirstOrDefaultAsync(ba => ba.Id == request.BankAccountId && ba.Company == request.Company, cancellationToken);
                    
                    if (bankAccount == null)
                    {
                        return Result.Failure<IncomeDto>("Bank account not found");
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
                        return Result.Failure<IncomeDto>("Counterparty not found");
                    }
                }

                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync(
                    "INCOME", 
                    request.Company, 
                    cancellationToken);

                // Calculate local amount
                var localAmount = request.Currency == "IRR" ? request.Amount : request.Amount * (request.ExchangeRate ?? 1);

                var income = new Income
                {
                    DocumentNumber = documentNumber,
                    Date = request.Date,
                    Description = request.Description,
                    Amount = request.Amount,
                    LocalAmount = localAmount,
                    Currency = request.Currency,
                    ExchangeRate = request.ExchangeRate ?? 1,
                    PaymentSource = PaymentSource.Bank,
                    BankAccountId = request.BankAccountId,
                    CounterpartyId = request.CounterpartyId,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    Status = IncomeStatus.Posted,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Incomes.Add(income);
                await _context.SaveChangesAsync(cancellationToken);

                // Create ledger entry if payment source is bank
                if (request.PaymentSource == PaymentSource.Bank && bankAccount != null)
                {
                    await CreateLedgerEntryAsync(income, bankAccount, cancellationToken);
                }

                return Result<IncomeDto>.Success(new IncomeDto
                {
                    Id = income.Id,
                    DocumentNumber = income.DocumentNumber,
                    Date = income.Date,
                    Description = income.Description,
                    Amount = income.Amount,
                    Currency = income.Currency,
                    ExchangeRate = income.ExchangeRate,
                    LocalAmount = income.LocalAmount,
                    PaymentSource = income.PaymentSource,
                    BankAccountId = income.BankAccountId,
                    CounterpartyId = income.CounterpartyId,
                    Reference = income.Reference,
                    Notes = income.Notes,
                    Status = income.Status,
                    Company = income.Company
                });
            }
            catch (Exception ex)
            {
                return Result.Failure<IncomeDto>($"Error creating income: {ex.Message}");
            }
        }

        private async Task CreateLedgerEntryAsync(Income income, BankAccount bankAccount, CancellationToken cancellationToken)
        {
            // Debit: Bank Account (increase balance for income)
            var ledgerEntry = new LedgerEntry
            {
                Date = income.Date,
                DocumentNumber = income.DocumentNumber,
                Description = income.Description,
                DebitAmount = income.Amount,
                CreditAmount = 0,
                Currency = income.Currency,
                ExchangeRate = income.ExchangeRate,
                LocalDebitAmount = income.LocalAmount,
                LocalCreditAmount = 0,
                Reference = income.Reference,
                Company = income.Company
            };

            _context.LedgerEntries.Add(ledgerEntry);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}