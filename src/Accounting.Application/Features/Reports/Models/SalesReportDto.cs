using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Models
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTickets { get; set; }
        public int IssuedTickets { get; set; }
        public int CancelledTickets { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageTicketValue { get; set; }
        public List<AirlineReportDto> TicketsByAirline { get; set; } = new();
        public List<DestinationReportDto> TicketsByDestination { get; set; } = new();
        public List<MonthlySummaryDto> MonthlySummary { get; set; } = new();
    }

    public class AirlineReportDto
    {
        public string Name { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }

    public class DestinationReportDto
    {
        public string Name { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }

    public class MonthlySummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
    }
}