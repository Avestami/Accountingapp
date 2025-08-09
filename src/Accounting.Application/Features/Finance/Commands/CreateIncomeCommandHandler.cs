using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Services;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Finance.Commands
{
    public class CreateIncomeCommandHandler : IRequestHandler<CreateIncomeCommand, Result<IncomeDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDocumentNumberService _documentNumberService;
        private readonly IFxFifoService _fxFifoService;

        public CreateIncomeCommandHandler(
            IAccountingDbContext context,
            IMapper mapper,
            IDocumentNumberService documentNumberService,
            IFxFifoService fxFifoService)
        {
            _context = context;
            _mapper = mapper;
            _documentNumberService = documentNumberService;
            _fxFifoService = fxFifoService;
        }

        public async Task<Result<IncomeDto>> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync(
                    "INCOME", request.Company, cancellationToken);

                // Calculate exchange rate and local amount
                decimal exchangeRate = request.ExchangeRate ?? 1;
                decimal localAmount = request.Amount * exchangeRate;

                // Handle FX lot creation for foreign currencies
                if (request.Currency != "IRR" && request.PaymentSource == PaymentSource.Bank)
                {
                    // Create FX lot for foreign currency income
                    await _fxFifoService.AddFxLotAsync(
                        request.Currency, 
                        request.Amount, 
                        exchangeRate, 
                        request.Company, 
                        $"Income {documentNumber}",
                        cancellationToken);
                }

                // Validate bank account if specified
                if (request.BankAccountId.HasValue)
                {
                    var bankAccountExists = await _context.BankAccounts
                        .AnyAsync(ba => ba.Id == request.BankAccountId.Value && ba.IsActive, cancellationToken);

                    if (!bankAccountExists)
                    {
                        return Result.Failure<IncomeDto>("Invalid or inactive bank account");
                    }
                }

                // Create income entity
                var income = new Income
                {
                    DocumentNumber = documentNumber,
                    Date = request.Date,
                    Description = request.Description,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ExchangeRate = exchangeRate,
                    LocalAmount = localAmount,
                    PaymentSource = request.PaymentSource,
                    BankAccountId = request.BankAccountId,
                    CounterpartyId = request.CounterpartyId,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    Status = IncomeStatus.Draft,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Incomes.Add(income);
                await _context.SaveChangesAsync(cancellationToken);

                // Create ledger entries
                await CreateLedgerEntriesAsync(income, cancellationToken);

                var incomeDto = _mapper.Map<IncomeDto>(income);
                return Result.Success(incomeDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<IncomeDto>($"Error creating income: {ex.Message}");
            }
        }

        private async Task CreateLedgerEntriesAsync(Income income, CancellationToken cancellationToken)
        {
            // Debit: Cash/Bank Account
            var cashEntry = new LedgerEntry
            {
                Date = income.Date,
                DocumentNumber = income.DocumentNumber,
                DocumentType = "INCOME",
                DocumentId = income.Id,
                Description = income.Description,
                AccountCode = income.PaymentSource == PaymentSource.Cash ? "1100" : "1200",
                AccountName = income.PaymentSource == PaymentSource.Cash ? "Cash" : "Bank Account",
                DebitAmount = income.LocalAmount,
                CreditAmount = 0,
                Currency = "IRR",
                ExchangeRate = 1,
                LocalDebitAmount = income.LocalAmount,
                LocalCreditAmount = 0,
                CounterpartyId = income.CounterpartyId,
                Reference = income.Reference,
                Company = income.Company
            };

            // Credit: Revenue Account
            var revenueEntry = new LedgerEntry
            {
                Date = income.Date,
                DocumentNumber = income.DocumentNumber,
                DocumentType = "INCOME",
                DocumentId = income.Id,
                Description = income.Description,
                AccountCode = "4000", // General Revenue Account
                AccountName = "General Revenue",
                DebitAmount = 0,
                CreditAmount = income.LocalAmount,
                Currency = "IRR",
                ExchangeRate = 1,
                LocalDebitAmount = 0,
                LocalCreditAmount = income.LocalAmount,
                CounterpartyId = income.CounterpartyId,
                Reference = income.Reference,
                Company = income.Company
            };

            _context.LedgerEntries.Add(cashEntry);
            _context.LedgerEntries.Add(revenueEntry);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}