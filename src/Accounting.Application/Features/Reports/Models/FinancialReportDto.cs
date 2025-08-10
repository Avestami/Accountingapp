using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Models
{
    public class FinancialReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal TotalIncome { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal NetProfit { get; set; }
        public List<FinancialReportItemDto> IncomeItems { get; set; } = new();
        public List<FinancialReportItemDto> CostItems { get; set; } = new();
        public List<MonthlyFinancialSummaryDto> MonthlySummary { get; set; } = new();
    }

    public class FinancialReportItemDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public int TransactionCount { get; set; }
    }

    public class MonthlyFinancialSummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Costs { get; set; }
        public decimal NetProfit { get; set; }
    }

    public class CurrentAssetsDto
    {
        public decimal Cash { get; set; }
        public decimal AccountsReceivable { get; set; }
        public decimal Inventory { get; set; }
        public decimal PrepaidExpenses { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCurrentAssets { get; set; }
    }

    public class NonCurrentAssetsDto
    {
        public decimal PropertyPlantEquipment { get; set; }
        public decimal IntangibleAssets { get; set; }
        public decimal LongTermInvestments { get; set; }
        public decimal Total { get; set; }
        public decimal TotalNonCurrentAssets { get; set; }
    }

    public class CurrentLiabilitiesDto
    {
        public decimal AccountsPayable { get; set; }
        public decimal ShortTermDebt { get; set; }
        public decimal AccruedExpenses { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCurrentLiabilities { get; set; }
    }

    public class EquityDto
    {
        public decimal ShareCapital { get; set; }
        public decimal RetainedEarnings { get; set; }
        public decimal Total { get; set; }
        public decimal TotalEquity { get; set; }
    }
}