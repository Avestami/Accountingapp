using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Services
{
    public interface IFxFifoService
    {
        Task<FxConsumptionResult> ConsumeFxAsync(string currency, decimal amount, string company, CancellationToken cancellationToken = default);
        Task<FxTransaction> AddFxLotAsync(string currency, decimal amount, decimal rate, string company, string reference, CancellationToken cancellationToken = default);
        Task<List<FxLot>> GetAvailableLotsAsync(string currency, string company, CancellationToken cancellationToken = default);
    }

    public class FxFifoService : IFxFifoService
    {
        private readonly IAccountingDbContext _context;

        public FxFifoService(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<FxConsumptionResult> ConsumeFxAsync(string currency, decimal amount, string company, CancellationToken cancellationToken = default)
        {
            if (currency == "IRR" || amount <= 0)
            {
                return new FxConsumptionResult
                {
                    IsSuccess = true,
                    ConsumedAmount = amount,
                    WeightedAverageRate = 1,
                    Consumptions = new List<FxConsumption>()
                };
            }

            // Get available lots in FIFO order (oldest first)
            var availableLots = await _context.FxTransactions
                .Where(fx => fx.Currency == currency && 
                           fx.Company == company && 
                           fx.RemainingAmount > 0 &&
                           fx.TransactionType == FxTransactionType.Purchase)
                .OrderBy(fx => fx.Date)
                .ThenBy(fx => fx.Id)
                .ToListAsync(cancellationToken);

            var totalAvailable = availableLots.Sum(lot => lot.RemainingAmount);
            if (totalAvailable < amount)
            {
                return new FxConsumptionResult
                {
                    IsSuccess = false,
                    Error = $"Insufficient {currency} balance. Available: {totalAvailable}, Required: {amount}"
                };
            }

            var consumptions = new List<FxConsumption>();
            var remainingToConsume = amount;
            decimal totalCost = 0;

            foreach (var lot in availableLots)
            {
                if (remainingToConsume <= 0) break;

                var consumeFromThisLot = Math.Min(remainingToConsume, lot.RemainingAmount);
                var costFromThisLot = consumeFromThisLot * lot.ExchangeRate;

                // Create consumption record
                var consumption = new FxConsumption
                {
                    FxTransactionId = lot.Id,
                    ConsumedAmount = consumeFromThisLot,
                    ConsumedRate = lot.ExchangeRate,
                    ConsumedCost = costFromThisLot,
                    Date = DateTime.UtcNow,
                    Company = company
                };

                _context.FxConsumptions.Add(consumption);
                consumptions.Add(consumption);

                // Update lot remaining amount
                lot.RemainingAmount -= consumeFromThisLot;
                lot.UpdatedAt = DateTime.UtcNow;

                totalCost += costFromThisLot;
                remainingToConsume -= consumeFromThisLot;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new FxConsumptionResult
            {
                IsSuccess = true,
                ConsumedAmount = amount,
                WeightedAverageRate = totalCost / amount,
                Consumptions = consumptions
            };
        }

        public async Task<FxTransaction> AddFxLotAsync(string currency, decimal amount, decimal rate, string company, string reference, CancellationToken cancellationToken = default)
        {
            var fxTransaction = new FxTransaction
            {
                Currency = currency,
                Amount = amount,
                RemainingAmount = amount,
                ExchangeRate = rate,
                TransactionType = FxTransactionType.Purchase,
                Date = DateTime.UtcNow,
                Reference = reference,
                Company = company,
                CreatedAt = DateTime.UtcNow
            };

            _context.FxTransactions.Add(fxTransaction);
            await _context.SaveChangesAsync(cancellationToken);

            return fxTransaction;
        }

        public async Task<List<FxLot>> GetAvailableLotsAsync(string currency, string company, CancellationToken cancellationToken = default)
        {
            var lots = await _context.FxTransactions
                .Where(fx => fx.Currency == currency && 
                           fx.Company == company && 
                           fx.RemainingAmount > 0 &&
                           fx.TransactionType == FxTransactionType.Purchase)
                .OrderBy(fx => fx.Date)
                .Select(fx => new FxLot
                {
                    Id = fx.Id,
                    Date = fx.Date,
                    Currency = fx.Currency,
                    OriginalAmount = fx.Amount,
                    RemainingAmount = fx.RemainingAmount,
                    ExchangeRate = fx.ExchangeRate,
                    Reference = fx.Reference
                })
                .ToListAsync(cancellationToken);

            return lots;
        }
    }

    public class FxConsumptionResult
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public decimal ConsumedAmount { get; set; }
        public decimal WeightedAverageRate { get; set; }
        public List<FxConsumption> Consumptions { get; set; } = new();
    }

    public class FxLot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal OriginalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string? Reference { get; set; }
    }
}