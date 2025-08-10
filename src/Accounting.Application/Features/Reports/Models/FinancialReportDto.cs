using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Models
{
    public class FinancialReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal NetProfit { get; set; }
        public List<FinancialItemDto> IncomeItems { get; set; } = new();
        public List<FinancialItemDto> CostItems { get; set; } = new();
        public List<FinancialMonthlySummaryDto> MonthlySummary { get; set; } = new();
    }

    public class FinancialItemDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class FinancialMonthlySummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Costs { get; set; }
        public decimal NetProfit { get; set; }
    }
}