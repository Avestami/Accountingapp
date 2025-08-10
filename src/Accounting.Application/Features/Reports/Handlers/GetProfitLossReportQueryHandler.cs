using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Features.Reports.Queries;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Application.Common.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Reports.Handlers
{
    public class GetProfitLossReportQueryHandler : IQueryHandler<GetProfitLossReportQuery, Result<ProfitLossReportDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetProfitLossReportQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProfitLossReportDto>> Handle(GetProfitLossReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get revenue accounts (Income type accounts)
                var revenueAccounts = await _context.Accounts
                    .Where(a => a.Type == AccountType.Revenue)
                    .ToListAsync(cancellationToken);

                // Get expense accounts (Expense type accounts)
                var expenseAccounts = await _context.Accounts
                    .Where(a => a.Type == AccountType.Expense)
                    .ToListAsync(cancellationToken);

                // Calculate revenue from vouchers
                var revenueVouchers = await _context.Vouchers
                    .Include(v => v.Entries)
                    .ThenInclude(ve => ve.Account)
                    .Where(v => v.VoucherDate >= request.StartDate && v.VoucherDate <= request.EndDate)
                    .Where(v => v.Entries.Any(ve => revenueAccounts.Contains(ve.Account)))
                    .ToListAsync(cancellationToken);

                // Calculate expenses from vouchers
                var expenseVouchers = await _context.Vouchers
                    .Include(v => v.Entries)
                    .ThenInclude(ve => ve.Account)
                    .Where(v => v.VoucherDate >= request.StartDate && v.VoucherDate <= request.EndDate)
                    .Where(v => v.Entries.Any(ve => expenseAccounts.Contains(ve.Account)))
                    .ToListAsync(cancellationToken);

                // Calculate revenue items
                var revenueItems = new List<ProfitLossItemDto>();
                decimal totalRevenue = 0;

                foreach (var account in revenueAccounts)
                {
                    var accountRevenue = revenueVouchers
                        .SelectMany(v => v.Entries)
                        .Where(ve => ve.AccountId == account.Id)
                        .Sum(ve => ve.TransactionType == TransactionType.Credit ? ve.Amount : -ve.Amount);

                    if (accountRevenue != 0)
                    {
                        revenueItems.Add(new ProfitLossItemDto
                        {
                            AccountName = account.AccountName,
                            AccountCode = account.AccountCode,
                            Amount = accountRevenue
                        });
                        totalRevenue += accountRevenue;
                    }
                }

                // Calculate expense items
                var expenseItems = new List<ProfitLossItemDto>();
                decimal totalExpenses = 0;

                foreach (var account in expenseAccounts)
                {
                    var accountExpense = expenseVouchers
                        .SelectMany(v => v.Entries)
                        .Where(ve => ve.AccountId == account.Id)
                        .Sum(ve => ve.TransactionType == TransactionType.Debit ? ve.Amount : -ve.Amount);

                    if (accountExpense != 0)
                    {
                        expenseItems.Add(new ProfitLossItemDto
                        {
                            AccountName = account.AccountName,
                            AccountCode = account.AccountCode,
                            Amount = accountExpense
                        });
                        totalExpenses += accountExpense;
                    }
                }

                // Calculate percentages
                foreach (var item in revenueItems)
                {
                    item.Percentage = totalRevenue > 0 ? (item.Amount / totalRevenue) * 100 : 0;
                }

                foreach (var item in expenseItems)
                {
                    item.Percentage = totalExpenses > 0 ? (item.Amount / totalExpenses) * 100 : 0;
                }

                var grossProfit = totalRevenue - totalExpenses;
                var netProfit = grossProfit; // Simplified - in real scenario, would include other income/expenses
                var profitMargin = totalRevenue > 0 ? (double)(netProfit / totalRevenue) * 100 : 0;

                var report = new ProfitLossReportDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalRevenue = totalRevenue,
                    TotalExpenses = totalExpenses,
                    GrossProfit = grossProfit,
                    NetProfit = netProfit,
                    ProfitMargin = profitMargin,
                    RevenueItems = revenueItems.OrderByDescending(x => x.Amount).ToList(),
                    ExpenseItems = expenseItems.OrderByDescending(x => x.Amount).ToList()
                };

                return Result<ProfitLossReportDto>.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<ProfitLossReportDto>($"Error generating profit & loss report: {ex.Message}");
            }
        }
    }
}