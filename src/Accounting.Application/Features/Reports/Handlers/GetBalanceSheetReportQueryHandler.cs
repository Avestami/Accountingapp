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
    public class GetBalanceSheetReportQueryHandler : IRequestHandler<GetBalanceSheetReportQuery, Result<BalanceSheetReportDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetBalanceSheetReportQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<BalanceSheetReportDto>> Handle(GetBalanceSheetReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all accounts
                var accounts = await _context.Accounts.ToListAsync(cancellationToken);

                // Get all voucher entries up to the specified date
                var voucherEntries = await _context.VoucherEntries
                    .Include(ve => ve.Voucher)
                    .Include(ve => ve.Account)
                    .Where(ve => ve.Voucher.Date <= request.AsOfDate)
                    .ToListAsync(cancellationToken);

                // Calculate account balances
                var accountBalances = accounts.ToDictionary(a => a.Id, a => 0m);

                foreach (var entry in voucherEntries)
                {
                    var account = accounts.First(a => a.Id == entry.AccountId);
                    var balance = account.Type switch
                    {
                        AccountType.Asset => entry.Debit - entry.Credit,
                        AccountType.Liability => entry.Credit - entry.Debit,
                        AccountType.Equity => entry.Credit - entry.Debit,
                        AccountType.Revenue => entry.Credit - entry.Debit,
                        AccountType.Expense => entry.Debit - entry.Credit,
                        _ => 0
                    };
                    accountBalances[entry.AccountId] += balance;
                }

                // Build Assets section
                var assetAccounts = accounts.Where(a => a.Type == AccountType.Asset).ToList();
                var assets = BuildBalanceSheetSection("Assets", assetAccounts, accountBalances);

                // Build Liabilities section
                var liabilityAccounts = accounts.Where(a => a.Type == AccountType.Liability).ToList();
                var liabilities = BuildBalanceSheetSection("Liabilities", liabilityAccounts, accountBalances);

                // Build Equity section
                var equityAccounts = accounts.Where(a => a.Type == AccountType.Equity).ToList();
                var equity = BuildBalanceSheetSection("Equity", equityAccounts, accountBalances);

                // Calculate retained earnings (simplified)
                var revenueAccounts = accounts.Where(a => a.Type == AccountType.Revenue).ToList();
                var expenseAccounts = accounts.Where(a => a.Type == AccountType.Expense).ToList();

                var totalRevenue = revenueAccounts.Sum(a => accountBalances[a.Id]);
                var totalExpenses = expenseAccounts.Sum(a => accountBalances[a.Id]);
                var retainedEarnings = totalRevenue - totalExpenses;

                // Add retained earnings to equity
                if (retainedEarnings != 0)
                {
                    var retainedEarningsSection = equity.FirstOrDefault(s => s.SectionName == "Equity") ?? 
                        new BalanceSheetSectionDto { SectionName = "Equity", Items = new List<BalanceSheetItemDto>() };

                    retainedEarningsSection.Items.Add(new BalanceSheetItemDto
                    {
                        AccountName = "Retained Earnings",
                        AccountCode = "RE",
                        Amount = retainedEarnings
                    });

                    retainedEarningsSection.TotalAmount += retainedEarnings;

                    if (!equity.Any(s => s.SectionName == "Equity"))
                    {
                        equity.Add(retainedEarningsSection);
                    }
                }

                var totalAssets = assets.Sum(s => s.TotalAmount);
                var totalLiabilities = liabilities.Sum(s => s.TotalAmount);
                var totalEquity = equity.Sum(s => s.TotalAmount);

                var report = new BalanceSheetReportDto
                {
                    AsOfDate = request.AsOfDate,
                    TotalAssets = totalAssets,
                    TotalLiabilities = totalLiabilities,
                    TotalEquity = totalEquity,
                    Assets = assets,
                    Liabilities = liabilities,
                    Equity = equity
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<BalanceSheetReportDto>($"Error generating balance sheet report: {ex.Message}");
            }
        }

        private List<BalanceSheetSectionDto> BuildBalanceSheetSection(
            string sectionName, 
            List<Domain.Entities.Account> accounts, 
            Dictionary<int, decimal> accountBalances)
        {
            var sections = new List<BalanceSheetSectionDto>();

            // Group accounts by parent (simplified - could be more sophisticated)
            var parentGroups = accounts
                .Where(a => accountBalances[a.Id] != 0)
                .GroupBy(a => a.ParentAccountId.HasValue ? accounts.FirstOrDefault(p => p.Id == a.ParentAccountId)?.AccountName ?? sectionName : sectionName);

            foreach (var group in parentGroups)
            {
                var items = group.Select(a => new BalanceSheetItemDto
                {
                    AccountName = a.AccountName,
                    AccountCode = a.AccountCode,
                    Amount = Math.Abs(accountBalances[a.Id]) // Show absolute values in balance sheet
                }).ToList();

                sections.Add(new BalanceSheetSectionDto
                {
                    SectionName = group.Key,
                    TotalAmount = items.Sum(i => i.Amount),
                    Items = items.OrderBy(i => i.AccountCode).ToList()
                });
            }

            return sections;
        }
    }
}