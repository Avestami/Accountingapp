using Accounting.Application.DTOs;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Tickets.Commands
{
    public class UpdateTicketCommand
    {
        public int Id { get; set; }
        public int CounterpartyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public TicketType Type { get; set; }
        public List<UpdateTicketItemDto> Items { get; set; } = new List<UpdateTicketItemDto>();
    }

    public class UpdateTicketItemDto
    {
        public int? Id { get; set; } // Null for new items
        public string PassengerName { get; set; } = string.Empty;
        public string? PassengerAge { get; set; }
        public int? AirlineId { get; set; }
        public int? OriginId { get; set; }
        public int? DestinationId { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string? FlightNumber { get; set; }
        public string? SeatNumber { get; set; }
        public string? Class { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public string? Notes { get; set; }
        public string Itinerary { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false; // For marking items to delete
    }
}