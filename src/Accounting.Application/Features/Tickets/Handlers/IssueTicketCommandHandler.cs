using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class IssueTicketCommandHandler : ICommandHandler<IssueTicketCommand, Result<TicketDto>>
    {
        private readonly IAccountingDbContext _context;

        public IssueTicketCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TicketDto>> Handle(IssueTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result.Failure<TicketDto>("Ticket not found");
                }

                // Check if ticket can be issued
                if (ticket.Status != TicketStatus.Draft)
                {
                    return Result.Failure<TicketDto>("Only draft tickets can be issued");
                }

                // Validate ticket has items
                if (!ticket.Items.Any())
                {
                    return Result.Failure<TicketDto>("Cannot issue ticket without items");
                }

                // Update ticket status
                ticket.Status = TicketStatus.Pending;
                ticket.UpdatedAt = DateTime.UtcNow;

                // Add notes if provided
                if (!string.IsNullOrEmpty(command.Notes))
                {
                    ticket.Description = string.IsNullOrEmpty(ticket.Description) 
                        ? $"Issue Notes: {command.Notes}"
                        : $"{ticket.Description}\nIssue Notes: {command.Notes}";
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Return updated DTO
                var ticketDto = await MapToDto(ticket);
                return Result<TicketDto>.Success(ticketDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<TicketDto>($"Error issuing ticket: {ex.Message}");
            }
        }

        private async Task<TicketDto> MapToDto(Domain.Entities.Ticket ticket)
        {
            // Load related entities
            var ticketWithIncludes = await _context.Tickets
                .Include(t => t.Counterparty)
                .Include(t => t.Items)
                    .ThenInclude(ti => ti.Airline)
                .Include(t => t.Items)
                    .ThenInclude(ti => ti.Origin)
                .Include(t => t.Items)
                    .ThenInclude(ti => ti.Destination)
                .FirstOrDefaultAsync(t => t.Id == ticket.Id);

            if (ticketWithIncludes == null)
                ticketWithIncludes = ticket;

            var dto = new TicketDto
            {
                Id = ticketWithIncludes.Id,
                TicketNumber = ticketWithIncludes.TicketNumber,
                Title = ticketWithIncludes.Title,
                Description = ticketWithIncludes.Description,
                Amount = ticketWithIncludes.Amount,
                Currency = ticketWithIncludes.Currency,
                Status = ticketWithIncludes.Status,
                Type = ticketWithIncludes.Type,
                CounterpartyId = ticketWithIncludes.CounterpartyId,
                CounterpartyName = ticketWithIncludes.Counterparty?.Name ?? "",
                CreatedAt = ticketWithIncludes.CreatedAt,
                ModifiedAt = ticketWithIncludes.UpdatedAt,
                CancellationReason = ticketWithIncludes.CancellationReason,
                Items = ticketWithIncludes.Items.Select(item => new TicketItemDto
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
            var earliestServiceDate = ticketWithIncludes.Items
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

            return dto;
        }
    }
}