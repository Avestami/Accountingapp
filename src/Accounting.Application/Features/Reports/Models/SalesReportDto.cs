using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Models
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public int IssuedTickets { get; set; }
        public int CancelledTickets { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageTicketValue { get; set; }
        public List<SalesReportItemDto> TicketsByAirline { get; set; } = new();
        public List<SalesReportItemDto> TicketsByDestination { get; set; } = new();
        public List<MonthlySalesSummaryDto> MonthlySummary { get; set; } = new();
    }

    public class SalesReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }

    public class MonthlySalesSummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal GrowthRate { get; set; }
    }
}