using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Queries;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class GetTicketByIdQueryHandler : IQueryHandler<GetTicketByIdQuery, Result<TicketDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetTicketByIdQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TicketDto>> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Counterparty)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Airline)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Origin)
                    .Include(t => t.Items)
                        .ThenInclude(ti => ti.Destination)
                    .FirstOrDefaultAsync(t => t.Id == query.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result.Failure<TicketDto>("Ticket not found");
                }

                var dto = new TicketDto
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
                    ModifiedAt = ticket.UpdatedAt,
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
                };

                // Calculate 5-day rule
                var earliestServiceDate = ticket.Items
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

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure<TicketDto>($"Error retrieving ticket: {ex.Message}");
            }
        }
    }
}