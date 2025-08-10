using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Interfaces;
using Accounting.Application.Features.Tickets.Queries;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class GetSalesStatisticsQueryHandler : IQueryHandler<GetSalesStatisticsQuery, Result<SalesStatisticsDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetSalesStatisticsQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<SalesStatisticsDto>> Handle(GetSalesStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tickets = await _context.Tickets
                    .Where(t => t.CreatedAt >= request.StartDate && t.CreatedAt <= request.EndDate)
                    .ToListAsync(cancellationToken);

                var totalTickets = tickets.Count;
                var issuedTickets = tickets.Count(t => t.Status == TicketStatus.Issued);
                var cancelledTickets = tickets.Count(t => t.Status == TicketStatus.Cancelled);
                var pendingTickets = tickets.Count(t => t.Status == TicketStatus.Pending);

                var totalRevenue = tickets.Where(t => t.Status == TicketStatus.Issued).Sum(t => t.TotalAmount);
                var averageTicketValue = issuedTickets > 0 ? totalRevenue / issuedTickets : 0;
                var cancellationRate = totalTickets > 0 ? (decimal)cancelledTickets / totalTickets * 100 : 0;

                // Top airlines
                var topAirlines = tickets
                    .Where(t => t.Status == TicketStatus.Issued && !string.IsNullOrEmpty(t.Airline))
                    .GroupBy(t => t.Airline)
                    .Select(g => new SalesStatisticsItemDto
                    {
                        Name = g.Key,
                        Count = g.Count(),
                        Revenue = g.Sum(t => t.TotalAmount),
                        Percentage = totalRevenue > 0 ? (g.Sum(t => t.TotalAmount) / totalRevenue) * 100 : 0
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(5)
                    .ToArray();

                // Top destinations
                var topDestinations = tickets
                    .Where(t => t.Status == TicketStatus.Issued && !string.IsNullOrEmpty(t.Route))
                    .GroupBy(t => t.Route)
                    .Select(g => new SalesStatisticsItemDto
                    {
                        Name = g.Key,
                        Count = g.Count(),
                        Revenue = g.Sum(t => t.TotalAmount),
                        Percentage = totalRevenue > 0 ? (g.Sum(t => t.TotalAmount) / totalRevenue) * 100 : 0
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(5)
                    .ToArray();

                // Monthly sales
                var monthlySales = tickets
                    .Where(t => t.Status == TicketStatus.Issued)
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(g => new MonthlySalesDto
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                        Year = g.Key.Year,
                        TicketCount = g.Count(),
                        Revenue = g.Sum(t => t.TotalAmount),
                        GrowthRate = 0 // Calculate growth rate if needed
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => DateTime.ParseExact(x.Month, "MMM", null).Month)
                    .ToArray();

                // Calculate growth rates
                for (int i = 1; i < monthlySales.Length; i++)
                {
                    var current = monthlySales[i];
                    var previous = monthlySales[i - 1];
                    
                    if (previous.Revenue > 0)
                    {
                        current.GrowthRate = ((current.Revenue - previous.Revenue) / previous.Revenue) * 100;
                    }
                }

                var statistics = new SalesStatisticsDto
                {
                    TotalTickets = totalTickets,
                    IssuedTickets = issuedTickets,
                    CancelledTickets = cancelledTickets,
                    PendingTickets = pendingTickets,
                    TotalRevenue = totalRevenue,
                    AverageTicketValue = averageTicketValue,
                    CancellationRate = cancellationRate,
                    TopAirlines = topAirlines,
                    TopDestinations = topDestinations,
                    MonthlySales = monthlySales
                };

                return Result<SalesStatisticsDto>.Success(statistics);
            }
            catch (Exception ex)
            {
                return Result<SalesStatisticsDto>.Failure($"Error generating sales statistics: {ex.Message}");
            }
        }
    }
}