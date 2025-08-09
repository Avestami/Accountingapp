using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Dashboard.Queries
{
    public class DashboardStatsDto
    {
        public SalesStatsDto SalesDocuments { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public int PendingVouchers { get; set; }
        public int ActiveUsers { get; set; }
        public List<ChartDataDto> RevenueChart { get; set; } = new();
        public List<ChartDataDto> SalesDistribution { get; set; } = new();
        public List<RecentActivityDto> RecentSales { get; set; } = new();
        public List<RecentActivityDto> RecentVouchers { get; set; } = new();
        public CurrencyBalanceDto CurrencyBalances { get; set; } = new();
    }

    public class SalesStatsDto
    {
        public int Total { get; set; }
        public int Issued { get; set; }
        public int Unissued { get; set; }
        public int Canceled { get; set; }
    }

    public class ChartDataDto
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string? Color { get; set; }
    }

    public class RecentActivityDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string Counterparty { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CurrencyBalanceDto
    {
        public decimal IRR { get; set; }
        public decimal USD { get; set; }
        public decimal EUR { get; set; }
        public decimal GBP { get; set; }
        public decimal AED { get; set; }
    }
}