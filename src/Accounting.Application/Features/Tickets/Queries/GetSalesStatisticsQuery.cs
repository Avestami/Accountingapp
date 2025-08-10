using System;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;

namespace Accounting.Application.Features.Tickets.Queries
{
    public class GetSalesStatisticsQuery : IQuery<Result<SalesStatisticsDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SalesStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int IssuedTickets { get; set; }
        public int CancelledTickets { get; set; }
        public int PendingTickets { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageTicketValue { get; set; }
        public decimal CancellationRate { get; set; }
        public SalesStatisticsItemDto[] TopAirlines { get; set; } = Array.Empty<SalesStatisticsItemDto>();
        public SalesStatisticsItemDto[] TopDestinations { get; set; } = Array.Empty<SalesStatisticsItemDto>();
        public MonthlySalesDto[] MonthlySales { get; set; } = Array.Empty<MonthlySalesDto>();
    }

    public class SalesStatisticsItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlySalesDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public int TicketCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal GrowthRate { get; set; }
    }
}