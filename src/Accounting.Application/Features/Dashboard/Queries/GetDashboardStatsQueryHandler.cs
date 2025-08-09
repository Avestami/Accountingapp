using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsQueryHandler : IQueryHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetDashboardStatsQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var fromDate = query.FromDate ?? DateTime.Now.AddDays(-query.Period);
                var toDate = query.ToDate ?? DateTime.Now;

                var stats = new DashboardStatsDto();

                // Get Sales Documents Statistics
                var salesDocuments = await _context.Tickets
                    .Where(t => t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
                    .ToListAsync(cancellationToken);

                stats.SalesDocuments = new SalesStatsDto
                {
                    Total = salesDocuments.Count,
                    Issued = salesDocuments.Count(t => t.Status == Domain.Enums.TicketStatus.Completed),
                    Unissued = salesDocuments.Count(t => t.Status == Domain.Enums.TicketStatus.Pending),
                    Canceled = salesDocuments.Count(t => t.Status == Domain.Enums.TicketStatus.Cancelled)
                };

                // Get Revenue Statistics
                var incomes = await _context.Incomes
                    .Where(i => i.CreatedAt >= fromDate && i.CreatedAt <= toDate)
                    .ToListAsync(cancellationToken);

                var currentRevenue = incomes.Sum(i => i.Amount);
                
                // Calculate previous period for growth comparison
                var previousFromDate = fromDate.AddDays(-(toDate - fromDate).Days);
                var previousIncomes = await _context.Incomes
                    .Where(i => i.CreatedAt >= previousFromDate && i.CreatedAt < fromDate)
                    .ToListAsync(cancellationToken);

                var previousRevenue = previousIncomes.Sum(i => i.Amount);
                
                stats.TotalRevenue = currentRevenue;
                stats.RevenueGrowth = previousRevenue > 0 
                    ? ((currentRevenue - previousRevenue) / previousRevenue) * 100 
                    : 0;

                // Get Pending Vouchers Count
                stats.PendingVouchers = await _context.Vouchers
                    .CountAsync(v => v.Status == Domain.Enums.VoucherStatus.Pending, cancellationToken);

                // Get Active Users Count (users who logged in within the last 30 days)
                stats.ActiveUsers = await _context.Users
                    .CountAsync(u => u.LastLoginAt >= DateTime.Now.AddDays(-30), cancellationToken);

                // Generate Revenue Chart Data (daily revenue for the period)
                stats.RevenueChart = await GenerateRevenueChartData(fromDate, toDate, cancellationToken);

                // Generate Sales Distribution Data
                stats.SalesDistribution = await GenerateSalesDistributionData(fromDate, toDate, cancellationToken);

                // Get Recent Sales
                stats.RecentSales = await GetRecentSales(cancellationToken);

                // Get Recent Vouchers
                stats.RecentVouchers = await GetRecentVouchers(cancellationToken);

                // Get Currency Balances
                stats.CurrencyBalances = await GetCurrencyBalances(cancellationToken);

                return Result.Success(stats);
            }
            catch (Exception ex)
            {
                return Result.Failure<DashboardStatsDto>($"Error retrieving dashboard statistics: {ex.Message}");
            }
        }

        private async Task<List<ChartDataDto>> GenerateRevenueChartData(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            var dailyRevenue = await _context.Incomes
                .Where(i => i.CreatedAt >= fromDate && i.CreatedAt <= toDate)
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new ChartDataDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Sum(i => i.Amount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync(cancellationToken);

            return dailyRevenue;
        }

        private async Task<List<ChartDataDto>> GenerateSalesDistributionData(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            var salesByType = await _context.Tickets
                .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
                .GroupBy(s => s.Type)
                .Select(g => new ChartDataDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync(cancellationToken);

            return salesByType;
        }

        private async Task<List<RecentActivityDto>> GetRecentSales(CancellationToken cancellationToken)
        {
            var recentSales = await _context.Tickets
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .Select(s => new RecentActivityDto
                {
                    Id = s.Id,
                    DocumentNumber = s.TicketNumber,
                    Counterparty = s.Title,
                    Amount = s.Amount,
                    Date = s.CreatedAt,
                    Status = s.Status.ToString()
                })
                .ToListAsync(cancellationToken);

            return recentSales;
        }

        private async Task<List<RecentActivityDto>> GetRecentVouchers(CancellationToken cancellationToken)
        {
            var recentVouchers = await _context.Vouchers
                .OrderByDescending(v => v.CreatedAt)
                .Take(5)
                .Select(v => new RecentActivityDto
                {
                    Id = v.Id,
                    DocumentNumber = v.VoucherNumber,
                    Counterparty = v.Description,
                    Amount = v.Amount,
                    Date = v.CreatedAt,
                    Status = v.Status.ToString()
                })
                .ToListAsync(cancellationToken);

            return recentVouchers;
        }

        private async Task<CurrencyBalanceDto> GetCurrencyBalances(CancellationToken cancellationToken)
        {
            // Get currency balances from FX transactions
            var fxBalances = await _context.FxTransactions
                .GroupBy(fx => fx.Currency)
                .Select(g => new { Currency = g.Key, Balance = g.Sum(fx => fx.Amount) })
                .ToListAsync(cancellationToken);

            var currencyBalance = new CurrencyBalanceDto();
            
            foreach (var balance in fxBalances)
            {
                switch (balance.Currency)
                {
                    case "USD":
                        currencyBalance.USD = balance.Balance;
                        break;
                    case "EUR":
                        currencyBalance.EUR = balance.Balance;
                        break;
                    case "IRR":
                        currencyBalance.IRR = balance.Balance;
                        break;
                }
            }

            return currencyBalance;
        }
    }
}