using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Application.Services;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Finance.Commands
{
    public class CreateCostCommandHandler : ICommandHandler<CreateCostCommand, Result<CostDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDocumentNumberService _documentNumberService;
        private readonly IFxFifoService _fxFifoService;

        public CreateCostCommandHandler(
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

        public async Task<Result<CostDto>> Handle(CreateCostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync(
                    "COST", request.Company, cancellationToken);

                // Calculate exchange rate and local amount
                decimal exchangeRate = request.ExchangeRate ?? 1;
                decimal localAmount = request.Amount * exchangeRate;

                // Handle FX consumption for foreign currencies
                if (request.Currency != "IRR" && request.PaymentSource == PaymentSource.Bank)
                {
                    var fxResult = await _fxFifoService.ConsumeFxAsync(
                        request.Currency, request.Amount, request.Company, cancellationToken);

                    if (!fxResult.IsSuccess)
                    {
                        return Result.Failure<CostDto>(fxResult.Error ?? "Insufficient foreign currency balance");
                    }

                    // Use weighted average rate from FIFO consumption
                    exchangeRate = fxResult.WeightedAverageRate;
                    localAmount = request.Amount * exchangeRate;
                }

                // Validate bank account if specified
                if (request.BankAccountId.HasValue)
                {
                    var bankAccountExists = await _context.BankAccounts
                        .AnyAsync(ba => ba.Id == request.BankAccountId.Value && ba.IsActive, cancellationToken);

                    if (!bankAccountExists)
                    {
                        return Result.Failure<CostDto>("Invalid or inactive bank account");
                    }
                }

                // Create cost entity
                var cost = new Cost
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
                    Status = CostStatus.Draft,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Costs.Add(cost);
                await _context.SaveChangesAsync(cancellationToken);

                // Create ledger entries
                await CreateLedgerEntriesAsync(cost, cancellationToken);

                var costDto = _mapper.Map<CostDto>(cost);
                return Result.Success(costDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<CostDto>($"Error creating cost: {ex.Message}");
            }
        }

        private async Task CreateLedgerEntriesAsync(Cost cost, CancellationToken cancellationToken)
        {
            // Debit: Expense Account
            var expenseEntry = new LedgerEntry
            {
                Date = cost.Date,
                DocumentNumber = cost.DocumentNumber,
                DocumentType = "COST",
                DocumentId = cost.Id,
                Description = cost.Description,
                AccountCode = "6000", // General Expense Account
                AccountName = "General Expenses",
                DebitAmount = cost.LocalAmount,
                CreditAmount = 0,
                Currency = "IRR",
                ExchangeRate = 1,
                LocalDebitAmount = cost.LocalAmount,
                LocalCreditAmount = 0,
                CounterpartyId = cost.CounterpartyId,
                Reference = cost.Reference,
                Company = cost.Company
            };

            // Credit: Cash/Bank Account
            var cashEntry = new LedgerEntry
            {
                Date = cost.Date,
                DocumentNumber = cost.DocumentNumber,
                DocumentType = "COST",
                DocumentId = cost.Id,
                Description = cost.Description,
                AccountCode = cost.PaymentSource == PaymentSource.Cash ? "1100" : "1200",
                AccountName = cost.PaymentSource == PaymentSource.Cash ? "Cash" : "Bank Account",
                DebitAmount = 0,
                CreditAmount = cost.LocalAmount,
                Currency = "IRR",
                ExchangeRate = 1,
                LocalDebitAmount = 0,
                LocalCreditAmount = cost.LocalAmount,
                CounterpartyId = cost.CounterpartyId,
                Reference = cost.Reference,
                Company = cost.Company
            };

            _context.LedgerEntries.Add(expenseEntry);
            _context.LedgerEntries.Add(cashEntry);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}