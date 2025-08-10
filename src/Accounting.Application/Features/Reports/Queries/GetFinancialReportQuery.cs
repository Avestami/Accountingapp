using Accounting.Application.Common.Models;
using MediatR;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetFinancialReportQuery : IRequest<Result<FinancialReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Currency { get; set; }
    }

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
}