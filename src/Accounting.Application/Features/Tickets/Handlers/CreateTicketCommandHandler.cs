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
    public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Result<TicketDto>>
    {
        private readonly IAccountingDbContext _context;

        public CreateTicketCommandHandler(IAccountingDbContext context)
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
                    return Result.Failure<TicketDto>("Counterparty not found");
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
                    Status = TicketStatus.Draft,
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
                        AirlineId = itemDto.AirlineId ?? 0,
                        OriginId = itemDto.OriginId ?? 0,
                        DestinationId = itemDto.DestinationId ?? 0,
                        ServiceDate = itemDto.ServiceDate,
                        FlightNumber = itemDto.FlightNumber,
                        SeatNumber = itemDto.SeatNumber,
                        Class = itemDto.Class,
                        Amount = itemDto.Amount,
                        Currency = itemDto.Currency,
                        Notes = itemDto.Notes
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
                return Result.Failure<TicketDto>($"Error creating ticket: {ex.Message}");
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