using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Result<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public CreateTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TicketDto>> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validate counterparty exists
                var counterparty = await _context.Counterparties
                    .FirstOrDefaultAsync(c => c.Id == command.CounterpartyId, cancellationToken);

                if (counterparty == null)
                {
                    return Result<TicketDto>.Failure("Counterparty not found");
                }

                // Generate ticket number
                var ticketNumber = await GenerateTicketNumber();

                // Create ticket entity
                var ticket = new Ticket
                {
                    TicketNumber = ticketNumber,
                    Title = command.Title,
                    Description = command.Description,
                    Amount = command.Amount,
                    Currency = command.Currency,
                    Status = TicketStatus.Unissued,
                    Type = command.Type,
                    CounterpartyId = command.CounterpartyId,
                    CreatedAt = DateTime.UtcNow
                };

                // Add ticket items
                foreach (var itemDto in command.Items)
                {
                    var ticketItem = new TicketItem
                    {
                        PassengerName = itemDto.PassengerName,
                        PassengerAge = itemDto.PassengerAge,
                        AirlineId = itemDto.AirlineId,
                        OriginId = itemDto.OriginId,
                        DestinationId = itemDto.DestinationId,
                        ServiceDate = itemDto.ServiceDate,
                        FlightNumber = itemDto.FlightNumber,
                        SeatNumber = itemDto.SeatNumber,
                        Class = itemDto.Class,
                        Amount = itemDto.Amount,
                        Currency = itemDto.Currency,
                        Notes = itemDto.Notes,
                        Itinerary = itemDto.Itinerary
                    };

                    ticket.Items.Add(ticketItem);
                }

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync(cancellationToken);

                // Return DTO
                var ticketDto = await MapToDto(ticket);
                return Result<TicketDto>.Success(ticketDto);
            }
            catch (Exception ex)
            {
                return Result<TicketDto>.Failure($"Error creating ticket: {ex.Message}");
            }
        }

        private async Task<string> GenerateTicketNumber()
        {
            var today = DateTime.Today;
            var prefix = $"TK{today:yyyyMMdd}";
            
            var lastTicket = await _context.Tickets
                .Where(t => t.TicketNumber.StartsWith(prefix))
                .OrderByDescending(t => t.TicketNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastTicket != null)
            {
                var lastSequence = lastTicket.TicketNumber.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out int parsed))
                {
                    sequence = parsed + 1;
                }
            }

            return $"{prefix}{sequence:D4}";
        }

        private async Task<TicketDto> MapToDto(Ticket ticket)
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