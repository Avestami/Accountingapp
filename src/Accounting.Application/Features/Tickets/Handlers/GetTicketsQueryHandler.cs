using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Queries;
using Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class GetTicketsQueryHandler : IQueryHandler<GetTicketsQuery, Result<PagedResult<TicketDto>>>
    {
        private readonly ApplicationDbContext _context;

        public GetTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<TicketDto>>> Handle(GetTicketsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var ticketsQuery = _context.Tickets
                    .Include(t => t.Counterparty)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Airline)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Origin)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Destination)
                    .AsQueryable();

                // Apply filters
                ticketsQuery = ApplyFilters(ticketsQuery, query);

                // Get total count before pagination
                var totalCount = await ticketsQuery.CountAsync(cancellationToken);

                // Apply sorting
                ticketsQuery = ApplySorting(ticketsQuery, query.SortBy, query.SortDirection);

                // Apply pagination
                var tickets = await ticketsQuery
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken);

                // Map to DTOs
                var ticketDtos = tickets.Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Amount = ticket.Amount,
                    Currency = ticket.Currency,
                    Status = ticket.Status,
                    Type = ticket.Type,
                    CounterpartyId = ticket.CounterpartyId,
                    CounterpartyName = ticket.Counterparty?.Name ?? "",
                    CreatedAt = ticket.CreatedAt,
                    ModifiedAt = ticket.ModifiedAt,
                    CancellationReason = ticket.CancellationReason,
                    Items = ticket.Items.Select(item => new TicketItemDto
                    {
                        Id = item.Id,
                        PassengerName = item.PassengerName,
                        PassengerAge = item.PassengerAge,
                        AirlineId = item.AirlineId,
                        AirlineName = item.Airline?.Name,
                        OriginId = item.OriginId,
                        OriginName = item.Origin?.Name,
                        DestinationId = item.DestinationId,
                        DestinationName = item.Destination?.Name,
                        ServiceDate = item.ServiceDate,
                        FlightNumber = item.FlightNumber,
                        SeatNumber = item.SeatNumber,
                        Class = item.Class,
                        Amount = item.Amount,
                        Currency = item.Currency,
                        Notes = item.Notes,
                        Itinerary = item.Itinerary
                    }).ToList()
                }).ToList();

                // Calculate 5-day rule for each ticket
                foreach (var dto in ticketDtos)
                {
                    var earliestServiceDate = dto.Items
                        .Where(i => i.ServiceDate.HasValue)
                        .Select(i => i.ServiceDate!.Value)
                        .DefaultIfEmpty(DateTime.MaxValue)
                        .Min();

                    if (earliestServiceDate != DateTime.MaxValue)
                    {
                        var daysToTravel = (earliestServiceDate.Date - DateTime.Today).Days;
                        dto.DaysToTravel = daysToTravel;
                        dto.IsWithinFiveDays = daysToTravel <= 5 && daysToTravel >= 0;
                    }
                }

                var pagedResult = new PagedResult<TicketDto>
                {
                    Items = ticketDtos,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                };

                return Result<PagedResult<TicketDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<TicketDto>>.Failure($"Error retrieving tickets: {ex.Message}");
            }
        }

        private IQueryable<Domain.Entities.Ticket> ApplyFilters(IQueryable<Domain.Entities.Ticket> query, GetTicketsQuery filters)
        {
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(t => 
                    t.TicketNumber.ToLower().Contains(searchTerm) ||
                    t.Title.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                    (t.Counterparty != null && t.Counterparty.Name.ToLower().Contains(searchTerm)));
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(t => t.Status == filters.Status.Value);
            }

            if (filters.Type.HasValue)
            {
                query = query.Where(t => t.Type == filters.Type.Value);
            }

            if (filters.CounterpartyId.HasValue)
            {
                query = query.Where(t => t.CounterpartyId == filters.CounterpartyId.Value);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filters.FromDate.Value);
            }

            if (filters.ToDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= filters.ToDate.Value);
            }

            if (!string.IsNullOrEmpty(filters.Currency))
            {
                query = query.Where(t => t.Currency == filters.Currency);
            }

            if (filters.MinAmount.HasValue)
            {
                query = query.Where(t => t.Amount >= filters.MinAmount.Value);
            }

            if (filters.MaxAmount.HasValue)
            {
                query = query.Where(t => t.Amount <= filters.MaxAmount.Value);
            }

            if (filters.IsWithinFiveDays.HasValue)
            {
                var today = DateTime.Today;
                var fiveDaysFromNow = today.AddDays(5);

                if (filters.IsWithinFiveDays.Value)
                {
                    query = query.Where(t => t.Items.Any(i => 
                        i.ServiceDate.HasValue && 
                        i.ServiceDate.Value.Date >= today && 
                        i.ServiceDate.Value.Date <= fiveDaysFromNow));
                }
                else
                {
                    query = query.Where(t => !t.Items.Any(i => 
                        i.ServiceDate.HasValue && 
                        i.ServiceDate.Value.Date >= today && 
                        i.ServiceDate.Value.Date <= fiveDaysFromNow));
                }
            }

            return query;
        }

        private IQueryable<Domain.Entities.Ticket> ApplySorting(IQueryable<Domain.Entities.Ticket> query, string sortBy, string sortDirection)
        {
            var isDescending = sortDirection.ToLower() == "desc";

            return sortBy.ToLower() switch
            {
                "ticketnumber" => isDescending ? query.OrderByDescending(t => t.TicketNumber) : query.OrderBy(t => t.TicketNumber),
                "title" => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "amount" => isDescending ? query.OrderByDescending(t => t.Amount) : query.OrderBy(t => t.Amount),
                "status" => isDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                "type" => isDescending ? query.OrderByDescending(t => t.Type) : query.OrderBy(t => t.Type),
                "counterparty" => isDescending ? query.OrderByDescending(t => t.Counterparty!.Name) : query.OrderBy(t => t.Counterparty!.Name),
                "modifiedat" => isDescending ? query.OrderByDescending(t => t.ModifiedAt) : query.OrderBy(t => t.ModifiedAt),
                _ => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };
        }
    }
}