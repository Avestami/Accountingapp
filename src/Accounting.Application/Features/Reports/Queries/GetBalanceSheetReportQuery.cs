using Accounting.Application.Common.Models;
using MediatR;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetBalanceSheetReportQuery : IRequest<Result<BalanceSheetReportDto>>
    {
        public DateTime AsOfDate { get; set; }
    }

    public class BalanceSheetReportDto
    {
        public DateTime AsOfDate { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public List<BalanceSheetSectionDto> Assets { get; set; } = new();
        public List<BalanceSheetSectionDto> Liabilities { get; set; } = new();
        public List<BalanceSheetSectionDto> Equity { get; set; } = new();
    }

    public class BalanceSheetSectionDto
    {
        public string SectionName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<BalanceSheetItemDto> Items { get; set; } = new();
    }

    public class BalanceSheetItemDto
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}