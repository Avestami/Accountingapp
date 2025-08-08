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
    public class UpdateTicketCommandHandler : ICommandHandler<UpdateTicketCommand, Result<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TicketDto>> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Find existing ticket
                var ticket = await _context.Tickets
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result<TicketDto>.Failure("Ticket not found");
                }

                // Check if ticket can be updated (only Unissued tickets can be updated)
                if (ticket.Status != TicketStatus.Unissued)
                {
                    return Result<TicketDto>.Failure("Only unissued tickets can be updated");
                }

                // Validate counterparty exists
                var counterparty = await _context.Counterparties
                    .FirstOrDefaultAsync(c => c.Id == command.CounterpartyId, cancellationToken);

                if (counterparty == null)
                {
                    return Result<TicketDto>.Failure("Counterparty not found");
                }

                // Update ticket properties
                ticket.Title = command.Title;
                ticket.Description = command.Description;
                ticket.Amount = command.Amount;
                ticket.Currency = command.Currency;
                ticket.Type = command.Type;
                ticket.CounterpartyId = command.CounterpartyId;
                ticket.ModifiedAt = DateTime.UtcNow;

                // Handle ticket items updates
                await UpdateTicketItems(ticket, command.Items, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                // Return updated DTO
                var ticketDto = await MapToDto(ticket);
                return Result<TicketDto>.Success(ticketDto);
            }
            catch (Exception ex)
            {
                return Result<TicketDto>.Failure($"Error updating ticket: {ex.Message}");
            }
        }

        private async Task UpdateTicketItems(Ticket ticket, List<UpdateTicketItemDto> itemDtos, CancellationToken cancellationToken)
        {
            // Get existing item IDs
            var existingItemIds = ticket.Items.Select(i => i.Id).ToHashSet();
            var updatedItemIds = itemDtos.Where(dto => dto.Id.HasValue && !dto.IsDeleted).Select(dto => dto.Id!.Value).ToHashSet();

            // Remove items that are not in the update list or marked as deleted
            var itemsToRemove = ticket.Items.Where(item => 
                !updatedItemIds.Contains(item.Id) || 
                itemDtos.Any(dto => dto.Id == item.Id && dto.IsDeleted)).ToList();

            foreach (var item in itemsToRemove)
            {
                ticket.Items.Remove(item);
                _context.TicketItems.Remove(item);
            }

            // Update existing items and add new ones
            foreach (var itemDto in itemDtos.Where(dto => !dto.IsDeleted))
            {
                if (itemDto.Id.HasValue)
                {
                    // Update existing item
                    var existingItem = ticket.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.PassengerName = itemDto.PassengerName;
                        existingItem.PassengerAge = itemDto.PassengerAge;
                        existingItem.AirlineId = itemDto.AirlineId;
                        existingItem.OriginId = itemDto.OriginId;
                        existingItem.DestinationId = itemDto.DestinationId;
                        existingItem.ServiceDate = itemDto.ServiceDate;
                        existingItem.FlightNumber = itemDto.FlightNumber;
                        existingItem.SeatNumber = itemDto.SeatNumber;
                        existingItem.Class = itemDto.Class;
                        existingItem.Amount = itemDto.Amount;
                        existingItem.Currency = itemDto.Currency;
                        existingItem.Notes = itemDto.Notes;
                        existingItem.Itinerary = itemDto.Itinerary;
                    }
                }
                else
                {
                    // Add new item
                    var newItem = new TicketItem
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

                    ticket.Items.Add(newItem);
                }
            }
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