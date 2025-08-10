using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Queries;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Reports.Handlers
{
    public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetSalesReportQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get tickets data
                var tickets = await _context.Tickets
                    .Include(t => t.Airline)
                    .Where(t => t.CreatedAt >= request.StartDate && t.CreatedAt <= request.EndDate)
                    .ToListAsync(cancellationToken);

                var totalTickets = tickets.Count;
                var issuedTickets = tickets.Count(t => t.Status == TicketStatus.Completed);
            var cancelledTickets = tickets.Count(t => t.Status == TicketStatus.Cancelled);
            var totalRevenue = tickets.Where(t => t.Status == TicketStatus.Completed).Sum(t => t.TotalAmount);
                var averageTicketValue = issuedTickets > 0 ? totalRevenue / issuedTickets : 0;

                // Group by airline
                var ticketsByAirline = tickets
                    .Where(t => t.Status == TicketStatus.Completed)
                    .GroupBy(t => t.Airline ?? "Unknown")
                    .Select(g => new SalesReportItemDto
                    {
                        Name = g.Key,
                        Count = g.Count(),
                        Revenue = g.Sum(t => t.TotalAmount),
                        Percentage = totalRevenue > 0 ? (g.Sum(t => t.TotalAmount) / totalRevenue) * 100 : 0
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

                // Group by destination (using route information if available)
                var ticketsByDestination = tickets
                    .Where(t => t.Status == TicketStatus.Completed)
                    .GroupBy(t => t.Route ?? "Unknown")
                    .Select(g => new SalesReportItemDto
                    {
                        Name = g.Key,
                        TicketCount = g.Count(),
                        Revenue = g.Sum(t => t.TotalAmount),
                        Percentage = totalRevenue > 0 ? (g.Sum(t => t.TotalAmount) / totalRevenue) * 100 : 0
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

                // Generate monthly summary
                var monthlySummary = new List<MonthlySalesSummaryDto>();
                var currentDate = new DateTime(request.StartDate.Year, request.StartDate.Month, 1);
                var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, 1);

                decimal previousMonthRevenue = 0;

                while (currentDate <= endDate)
                {
                    var monthStart = currentDate;
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthlyTickets = tickets
                        .Where(t => t.CreatedAt >= monthStart && t.CreatedAt <= monthEnd && t.Status == TicketStatus.Completed)
                        .ToList();

                    var monthlyRevenue = monthlyTickets.Sum(t => t.TotalAmount);
                    var growthRate = previousMonthRevenue > 0 ? ((monthlyRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 : 0;

                    monthlySummary.Add(new MonthlySalesSummaryDto
                    {
                        Month = currentDate.ToString("yyyy-MM"),
                        TicketCount = monthlyTickets.Count,
                        Revenue = monthlyRevenue,
                        GrowthRate = growthRate
                    });

                    previousMonthRevenue = monthlyRevenue;
                    currentDate = currentDate.AddMonths(1);
                }

                var report = new SalesReportDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ReportType = request.ReportType,
                    TotalTickets = totalTickets,
                    IssuedTickets = issuedTickets,
                    CancelledTickets = cancelledTickets,
                    TotalRevenue = totalRevenue,
                    AverageTicketValue = averageTicketValue,
                    TicketsByAirline = ticketsByAirline,
                    TicketsByDestination = ticketsByDestination,
                    MonthlySummary = monthlySummary
                };

                return Result.Success(report);
            }
            catch (Exception ex)
            {
                return Result.Failure<SalesReportDto>($"Error generating sales report: {ex.Message}");
            }
        }
    }
}