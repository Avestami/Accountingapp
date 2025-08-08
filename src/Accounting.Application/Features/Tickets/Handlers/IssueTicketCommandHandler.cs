using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Domain.Enums;
using Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class IssueTicketCommandHandler : ICommandHandler<IssueTicketCommand, Result<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public IssueTicketCommandHandler(ApplicationDbContext context)
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
                    return Result<TicketDto>.Failure("Ticket not found");
                }

                // Check if ticket can be issued
                if (ticket.Status != TicketStatus.Unissued)
                {
                    return Result<TicketDto>.Failure("Only unissued tickets can be issued");
                }

                // Validate ticket has items
                if (!ticket.Items.Any())
                {
                    return Result<TicketDto>.Failure("Cannot issue ticket without items");
                }

                // Update ticket status
                ticket.Status = TicketStatus.Issued;
                ticket.ModifiedAt = DateTime.UtcNow;

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
                return Result<TicketDto>.Failure($"Error issuing ticket: {ex.Message}");
            }
        }

        private async Task<TicketDto> MapToDto(Domain.Entities.Ticket ticket)
        {
            await _context.Entry(ticket)
                .Reference(t => t.Counterparty)
                .LoadAsync();

            await _context.Entry(ticket)
                .Collection(t => t.Items)
                .Query()
                .Include(ti => ti.Airline)
                .Include(ti => ti.Origin)
                .Include(ti => ti.Destination)
                .LoadAsync();

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

            return dto;
        }
    }
}