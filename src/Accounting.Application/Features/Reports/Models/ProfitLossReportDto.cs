using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Models
{
    public class ProfitLossReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RevenueBreakdownDto Revenue { get; set; } = new();
        public List<ExpenseItemDto> Expenses { get; set; } = new();
        public decimal TotalExpenses { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public double ProfitMargin { get; set; }
    }

    public class RevenueBreakdownDto
    {
        public decimal TicketSales { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ExpenseItemDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
    }
}