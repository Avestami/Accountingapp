using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Queries;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Reports.Handlers
{
    public class GetFinancialReportQueryHandler : IRequestHandler<GetFinancialReportQuery, Result<FinancialReportDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetFinancialReportQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<FinancialReportDto>> Handle(GetFinancialReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var currency = request.Currency ?? "USD";

                // Get income data
                var incomes = await _context.Incomes
                    .Where(i => i.Date >= request.StartDate && i.Date <= request.EndDate)
                    .Where(i => string.IsNullOrEmpty(request.Currency) || i.Currency == currency)
                    .ToListAsync(cancellationToken);

                // Get cost data
                var costs = await _context.Costs
                    .Where(c => c.Date >= request.StartDate && c.Date <= request.EndDate)
                    .Where(c => string.IsNullOrEmpty(request.Currency) || c.Currency == currency)
                    .ToListAsync(cancellationToken);

                var totalIncome = incomes.Sum(i => i.Amount);
                var totalCosts = costs.Sum(c => c.Amount);

                // Group income by category
                var incomeItems = incomes
                    .GroupBy(i => i.Category ?? "Other")
                    .Select(g => new FinancialReportItemDto
                    {
                        Category = g.Key,
                        Amount = g.Sum(i => i.Amount),
                        Currency = currency,
                        TransactionCount = g.Count()
                    })
                    .ToList();

                // Group costs by category
                var costItems = costs
                    .GroupBy(c => c.Category ?? "Other")
                    .Select(g => new FinancialReportItemDto
                    {
                        Category = g.Key,
                        Amount = g.Sum(c => c.Amount),
                        Currency = currency,
                        TransactionCount = g.Count()
                    })
                    .ToList();

                // Generate monthly summary
                var monthlySummary = new List<MonthlyFinancialSummaryDto>();
                var currentDate = new DateTime(request.StartDate.Year, request.StartDate.Month, 1);
                var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, 1);

                while (currentDate <= endDate)
                {
                    var monthStart = currentDate;
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthlyIncome = incomes
                        .Where(i => i.Date >= monthStart && i.Date <= monthEnd)
                        .Sum(i => i.Amount);

                    var monthlyCosts = costs
                        .Where(c => c.Date >= monthStart && c.Date <= monthEnd)
                        .Sum(c => c.Amount);

                    monthlySummary.Add(new MonthlyFinancialSummaryDto
                    {
                        Month = currentDate.ToString("yyyy-MM"),
                        Income = monthlyIncome,
                        Costs = monthlyCosts,
                        NetProfit = monthlyIncome - monthlyCosts
                    });

                    currentDate = currentDate.AddMonths(1);
                }

                var report = new FinancialReportDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Currency = currency,
                    TotalIncome = totalIncome,
                    TotalCosts = totalCosts,
                    NetProfit = totalIncome - totalCosts,
                    IncomeItems = incomeItems,
                    CostItems = costItems,
                    MonthlySummary = monthlySummary
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<FinancialReportDto>($"Error generating financial report: {ex.Message}");
            }
        }
    }
}