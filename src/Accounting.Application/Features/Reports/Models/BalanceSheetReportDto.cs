using System;

namespace Accounting.Application.Features.Reports.Models
{
    public class BalanceSheetReportDto
    {
        public DateTime AsOfDate { get; set; }
        public AssetsDto Assets { get; set; } = new();
        public LiabilitiesDto Liabilities { get; set; } = new();
        public EquityDto Equity { get; set; } = new();
    }

    public class AssetsDto
    {
        public CurrentAssetsDto CurrentAssets { get; set; } = new();
        public decimal TotalAssets { get; set; }
    }

    public class CurrentAssetsDto
    {
        public decimal Cash { get; set; }
        public decimal AccountsReceivable { get; set; }
        public decimal TotalCurrentAssets { get; set; }
    }

    public class LiabilitiesDto
    {
        public CurrentLiabilitiesDto CurrentLiabilities { get; set; } = new();
        public decimal TotalLiabilities { get; set; }
    }

    public class CurrentLiabilitiesDto
    {
        public decimal AccountsPayable { get; set; }
        public decimal TotalCurrentLiabilities { get; set; }
    }

    public class EquityDto
    {
        public decimal RetainedEarnings { get; set; }
        public decimal TotalEquity { get; set; }
    }
}