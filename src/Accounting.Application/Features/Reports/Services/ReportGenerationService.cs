using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Accounting.Application.Features.Reports.Services
{
    public class ReportGenerationService : IReportGenerationService
    {
        private readonly IAccountingDbContext _context;

        public ReportGenerationService(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<SalesReportDto>> GenerateSalesReportAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Validate();

                var query = _context.Tickets.AsQueryable();

                // Apply filters
                query = ApplyDateFilter(query, t => t.RequestDate, filter.StartDate, filter.EndDate);
                query = ApplySearchFilter(query, filter.SearchTerm);
                query = ApplyAirlineFilter(query, filter.Airlines);
                query = ApplyDestinationFilter(query, filter.Destinations);
                query = ApplyStatusFilter(query, filter.Statuses);

                var tickets = await query.ToListAsync(cancellationToken);

                var report = new SalesReportDto
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    TotalTickets = tickets.Count,
                    IssuedTickets = tickets.Count(t => t.Status == TicketStatus.Completed),
                CancelledTickets = tickets.Count(t => t.Status == TicketStatus.Cancelled),
                TotalRevenue = tickets.Where(t => t.Status == TicketStatus.Completed).Sum(t => t.Amount),
                AverageTicketValue = tickets.Any(t => t.Status == TicketStatus.Completed)
                    ? tickets.Where(t => t.Status == TicketStatus.Completed).Average(t => t.Amount)
                    : 0,
                    TicketsByAirline = await GetTicketsByAirlineAsync(tickets, cancellationToken),
                    TicketsByDestination = await GetTicketsByDestinationAsync(tickets, cancellationToken),
                    MonthlySummary = GetMonthlySummary(tickets, filter.StartDate, filter.EndDate)
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<SalesReportDto>($"Failed to generate sales report: {ex.Message}");
            }
        }

        public async Task<Result<FinancialReportDto>> GenerateFinancialReportAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Validate();

                // Get income data
                var incomeQuery = _context.Incomes.AsQueryable();
                incomeQuery = ApplyDateFilter(incomeQuery, i => i.Date, filter.StartDate, filter.EndDate);
                incomeQuery = ApplyCategoryFilter(incomeQuery, i => i.Description, filter.Categories);
                var incomes = await incomeQuery.ToListAsync(cancellationToken);

                // Get cost data
                var costQuery = _context.Costs.AsQueryable();
                costQuery = ApplyDateFilter(costQuery, c => c.Date, filter.StartDate, filter.EndDate);
                costQuery = ApplyCategoryFilter(costQuery, c => c.Description, filter.Categories);
                var costs = await costQuery.ToListAsync(cancellationToken);

                var totalIncome = incomes.Sum(i => i.Amount);
                var totalCosts = costs.Sum(c => c.Amount);

                var report = new FinancialReportDto
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    TotalIncome = (decimal)totalIncome,
                    TotalCosts = (decimal)totalCosts,
                    NetProfit = totalIncome - totalCosts,
                    IncomeItems = incomes.GroupBy(i => i.Description)
                        .Select(g => new FinancialReportItemDto
                        {
                            Category = g.Key,
                            Amount = g.Sum(i => i.Amount),
                            TransactionCount = g.Count()
                        }).ToList(),
                    CostItems = costs.GroupBy(c => c.Description)
                        .Select(g => new FinancialReportItemDto
                        {
                            Category = g.Key,
                            Amount = g.Sum(c => c.Amount),
                            TransactionCount = g.Count()
                        }).ToList(),
                    MonthlySummary = GetFinancialMonthlySummary(incomes, costs, filter.StartDate, filter.EndDate)
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<FinancialReportDto>($"Failed to generate financial report: {ex.Message}");
            }
        }

        public async Task<Result<ProfitLossReportDto>> GenerateProfitLossReportAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Validate();

                // Get revenue from tickets
                var ticketQuery = _context.Tickets.Where(t => t.Status == TicketStatus.Completed);
                ticketQuery = ApplyDateFilter(ticketQuery, t => t.RequestDate, filter.StartDate, filter.EndDate);
                var ticketRevenue = await ticketQuery.SumAsync(t => t.Amount, cancellationToken);

                // Get other income
                var incomeQuery = _context.Incomes.AsQueryable();
                incomeQuery = ApplyDateFilter(incomeQuery, i => i.Date, filter.StartDate, filter.EndDate);
                var otherIncome = await incomeQuery.SumAsync(i => i.Amount, cancellationToken);

                // Get costs
                var costQuery = _context.Costs.AsQueryable();
                costQuery = ApplyDateFilter(costQuery, c => c.Date, filter.StartDate, filter.EndDate);
                var costs = await costQuery.ToListAsync(cancellationToken);

                var totalRevenue = ticketRevenue + otherIncome;
                var totalCosts = costs.Sum(c => c.Amount);

                var report = new ProfitLossReportDto
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    Revenue = new RevenueBreakdownDto
                    {
                        TicketSales = ticketRevenue,
                        OtherIncome = otherIncome,
                        TotalRevenue = totalRevenue
                    },
                    Expenses = costs.GroupBy(c => c.Description ?? "Other")
                        .Select(g => new ExpenseItemDto
                        {
                            Category = g.Key,
                            Amount = g.Sum(c => c.Amount),
                            Percentage = totalCosts > 0 ? (double)g.Sum(c => c.Amount) / (double)totalCosts * 100 : 0
                        }).ToList(),
                    TotalExpenses = totalCosts,
                    GrossProfit = totalRevenue - totalCosts,
                    NetProfit = totalRevenue - totalCosts, // Simplified for now
                    ProfitMargin = totalRevenue > 0 ? (double)(totalRevenue - totalCosts) / (double)totalRevenue * 100 : 0
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<ProfitLossReportDto>($"Failed to generate profit/loss report: {ex.Message}");
            }
        }

        public async Task<Result<BalanceSheetReportDto>> GenerateBalanceSheetReportAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Validate();

                // Get bank account balances using OpeningBalance
                var bankAccounts = await _context.BankAccounts.ToListAsync(cancellationToken);
                var totalCash = bankAccounts.Sum(ba => ba.OpeningBalance);

                // Get vouchers (accounts receivable/payable)
                var voucherQuery = _context.Vouchers.AsQueryable();
                voucherQuery = voucherQuery.Where(v => v.VoucherDate <= filter.EndDate);
                var vouchers = await voucherQuery.ToListAsync(cancellationToken);

                var accountsReceivable = vouchers.Where(v => v.Type == VoucherType.Income).Sum(v => v.Amount);
                var accountsPayable = vouchers.Where(v => v.Type == VoucherType.Expense).Sum(v => v.Amount);

                // Calculate equity (simplified)
                var totalAssets = totalCash + accountsReceivable;
                var totalLiabilities = accountsPayable;
                var equity = totalAssets - totalLiabilities;

                var report = new BalanceSheetReportDto
                {
                    AsOfDate = filter.EndDate,
                    TotalAssets = totalAssets,
                    TotalLiabilities = totalLiabilities,
                    TotalEquity = equity,
                    Assets = new List<BalanceSheetSectionDto>
                    {
                        new BalanceSheetSectionDto
                        {
                            SectionName = "Current Assets",
                            TotalAmount = totalCash + accountsReceivable,
                            Items = new List<BalanceSheetItemDto>
                            {
                                new BalanceSheetItemDto { AccountName = "Cash", Amount = totalCash },
                                new BalanceSheetItemDto { AccountName = "Accounts Receivable", Amount = accountsReceivable }
                            }
                        }
                    },
                    Liabilities = new List<BalanceSheetSectionDto>
                    {
                        new BalanceSheetSectionDto
                        {
                            SectionName = "Current Liabilities",
                            TotalAmount = accountsPayable,
                            Items = new List<BalanceSheetItemDto>
                            {
                                new BalanceSheetItemDto { AccountName = "Accounts Payable", Amount = accountsPayable }
                            }
                        }
                    },
                    Equity = new List<BalanceSheetSectionDto>
                    {
                        new BalanceSheetSectionDto
                        {
                            SectionName = "Equity",
                            TotalAmount = equity,
                            Items = new List<BalanceSheetItemDto>
                            {
                                new BalanceSheetItemDto { AccountName = "Retained Earnings", Amount = equity }
                            }
                        }
                    }
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<BalanceSheetReportDto>($"Failed to generate balance sheet report: {ex.Message}");
            }
        }

        public async Task<Result<CashFlowReportDto>> GenerateCashFlowReportAsync(ReportFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Validate();

                // Operating activities
                var ticketQuery = _context.Tickets.Where(t => t.Status == TicketStatus.Completed);
                ticketQuery = ApplyDateFilter(ticketQuery, t => t.RequestDate, filter.StartDate, filter.EndDate);
                var cashFromTickets = await ticketQuery.SumAsync(t => t.Amount, cancellationToken);

                var incomeQuery = _context.Incomes.AsQueryable();
                incomeQuery = ApplyDateFilter(incomeQuery, i => i.Date, filter.StartDate, filter.EndDate);
                var otherIncome = await incomeQuery.SumAsync(i => i.Amount, cancellationToken);

                var costQuery = _context.Costs.AsQueryable();
                costQuery = ApplyDateFilter(costQuery, c => c.Date, filter.StartDate, filter.EndDate);
                var operatingExpenses = await costQuery.SumAsync(c => c.Amount, cancellationToken);

                var netCashFromOperations = cashFromTickets + otherIncome - operatingExpenses;

                var report = new CashFlowReportDto
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    OperatingActivities = new OperatingActivitiesDto
                    {
                        CashFromTicketSales = cashFromTickets,
                        CashFromOtherIncome = otherIncome,
                        CashPaidForExpenses = -operatingExpenses,
                        NetCashFromOperations = netCashFromOperations
                    },
                    InvestingActivities = new InvestingActivitiesDto
                    {
                        NetCashFromInvesting = 0 // Simplified for now
                    },
                    FinancingActivities = new FinancingActivitiesDto
                    {
                        NetCashFromFinancing = 0 // Simplified for now
                    },
                    NetCashFlow = netCashFromOperations,
                    BeginningCashBalance = 0, // Would need historical data
                    EndingCashBalance = netCashFromOperations
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<CashFlowReportDto>($"Failed to generate cash flow report: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<T>>> GetPagedReportDataAsync<T>(IQueryable<T> query, ReportFilterDto filter, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                filter.Validate();

                var totalCount = await query.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

                var items = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync(cancellationToken);

                var result = new PagedResult<T>(
                    items,
                    totalCount,
                    filter.PageNumber,
                    filter.PageSize
                );

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<PagedResult<T>>($"Failed to get paged report data: {ex.Message}");
            }
        }

        private IQueryable<T> ApplyDateFilter<T>(IQueryable<T> query, Expression<Func<T, DateTime>> dateSelector, DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom.HasValue)
            {
                var fromPredicate = Expression.Lambda<Func<T, bool>>(
                    Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(dateFrom.Value)),
                    dateSelector.Parameters);
                query = query.Where(fromPredicate);
            }

            if (dateTo.HasValue)
            {
                var toPredicate = Expression.Lambda<Func<T, bool>>(
                    Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(dateTo.Value)),
                    dateSelector.Parameters);
                query = query.Where(toPredicate);
            }

            return query;
        }

        private IQueryable<Ticket> ApplySearchFilter(IQueryable<Ticket> query, string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => 
                    t.TicketNumber.Contains(searchTerm) ||
                    t.Title.Contains(searchTerm) ||
                    t.Description.Contains(searchTerm));
            }
            return query;
        }

        private IQueryable<Ticket> ApplyAirlineFilter(IQueryable<Ticket> query, List<string> airlines)
        {
            if (airlines?.Any() == true)
            {
                query = query.Where(t => t.Items.Any(item => airlines.Contains(item.Airline.Name)));
            }
            return query;
        }

        private IQueryable<Ticket> ApplyDestinationFilter(IQueryable<Ticket> query, List<string> destinations)
        {
            if (destinations?.Any() == true)
            {
                query = query.Where(t => t.Items.Any(item => destinations.Contains(item.Destination.Name)));
            }
            return query;
        }

        private IQueryable<Ticket> ApplyStatusFilter(IQueryable<Ticket> query, List<string> statusFilters)
        {
            if (statusFilters?.Any() == true)
            {
                query = query.Where(t => statusFilters.Contains(t.Status.ToString()));
            }
            return query;
        }

        private IQueryable<T> ApplyCategoryFilter<T>(IQueryable<T> query, Expression<Func<T, string>> categorySelector, List<string> categories)
        {
            if (categories?.Any() == true)
            {
                var parameter = categorySelector.Parameters[0];
                var containsMethod = typeof(List<string>).GetMethod("Contains");
                var containsExpression = Expression.Call(
                    Expression.Constant(categories),
                    containsMethod,
                    categorySelector.Body);
                var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
                query = query.Where(lambda);
            }
            return query;
        }

        private async Task<List<SalesReportItemDto>> GetTicketsByAirlineAsync(List<Ticket> tickets, CancellationToken cancellationToken)
        {
            var totalRevenue = tickets.Where(t => t.Status == TicketStatus.Completed).Sum(t => t.Amount);
            
            return tickets
                .Where(t => t.Status == TicketStatus.Completed)
                .SelectMany(t => t.Items)
                .GroupBy(item => item.Airline.Name)
                .Select(g => new SalesReportItemDto
                {
                    Name = g.Key,
                    TicketCount = g.Count(),
                    Revenue = g.Sum(item => item.Amount),
                    Percentage = totalRevenue > 0 ? (double)(g.Sum(item => item.Amount) / totalRevenue) * 100 : 0
                })
                .OrderByDescending(a => a.Revenue)
                .ToList();
        }

        private async Task<List<SalesReportItemDto>> GetTicketsByDestinationAsync(List<Ticket> tickets, CancellationToken cancellationToken)
        {
            var totalRevenue = tickets.Where(t => t.Status == TicketStatus.Completed).Sum(t => t.Amount);
            
            return tickets
                .Where(t => t.Status == TicketStatus.Completed)
                .SelectMany(t => t.Items)
                .GroupBy(item => item.Destination.Name)
                .Select(g => new SalesReportItemDto
                {
                    Name = g.Key,
                    TicketCount = g.Count(),
                    Revenue = g.Sum(item => item.Amount),
                    Percentage = totalRevenue > 0 ? (double)(g.Sum(item => item.Amount) / totalRevenue) * 100 : 0
                })
                .OrderByDescending(d => d.Revenue)
                .ToList();
        }

        private List<MonthlySalesSummaryDto> GetMonthlySummary(List<Ticket> tickets, DateTime? dateFrom, DateTime? dateTo)
        {
            var startDate = dateFrom ?? tickets.Min(t => t.RequestDate);
            var endDate = dateTo ?? tickets.Max(t => t.RequestDate);

            var monthlyData = new List<MonthlySalesSummaryDto>();
            var current = new DateTime(startDate.Year, startDate.Month, 1);

            while (current <= endDate)
            {
                var monthTickets = tickets.Where(t => 
                    t.RequestDate.Year == current.Year && 
                    t.RequestDate.Month == current.Month &&
                    t.Status == TicketStatus.Completed).ToList();

                monthlyData.Add(new MonthlySalesSummaryDto
                {
                    Month = current.ToString("yyyy-MM"),
                    TicketCount = monthTickets.Count,
                    Revenue = monthTickets.Sum(t => t.Amount)
                });

                current = current.AddMonths(1);
            }

            return monthlyData;
        }

        private List<MonthlyFinancialSummaryDto> GetFinancialMonthlySummary(List<Income> incomes, List<Cost> costs, DateTime? dateFrom, DateTime? dateTo)
        {
            var allDates = incomes.Select(i => i.Date).Concat(costs.Select(c => c.Date)).ToList();
            if (!allDates.Any()) return new List<MonthlyFinancialSummaryDto>();

            var startDate = dateFrom ?? allDates.Min();
            var endDate = dateTo ?? allDates.Max();

            var monthlyData = new List<MonthlyFinancialSummaryDto>();
            var current = new DateTime(startDate.Year, startDate.Month, 1);

            while (current <= endDate)
            {
                var monthIncomes = incomes.Where(i => 
                    i.Date.Year == current.Year && 
                    i.Date.Month == current.Month).Sum(i => i.Amount);

                var monthCosts = costs.Where(c => 
                    c.Date.Year == current.Year && 
                    c.Date.Month == current.Month).Sum(c => c.Amount);

                monthlyData.Add(new MonthlyFinancialSummaryDto
                {
                    Month = current.ToString("yyyy-MM"),
                    Income = monthIncomes,
                    Costs = monthCosts,
                    NetProfit = monthIncomes - monthCosts
                });

                current = current.AddMonths(1);
            }

            return monthlyData;
        }
    }
}