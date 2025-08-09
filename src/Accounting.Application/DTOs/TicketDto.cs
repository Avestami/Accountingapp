using System;
using System.Collections.Generic;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public TicketStatus Status { get; set; }
        public TicketType Type { get; set; }
        public int CounterpartyId { get; set; }
        public string CounterpartyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? CancellationReason { get; set; }
        public List<TicketItemDto> Items { get; set; } = new List<TicketItemDto>();
        
        // Computed properties
        public bool IsWithinFiveDays { get; set; }
        public int DaysToTravel { get; set; }
    }

    public class TicketItemDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string? PassengerAge { get; set; }
        public int? AirlineId { get; set; }
        public string? AirlineName { get; set; }
        public int? OriginId { get; set; }
        public string? OriginName { get; set; }
        public int? DestinationId { get; set; }
        public string? DestinationName { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string? FlightNumber { get; set; }
        public string? SeatNumber { get; set; }
        public string? Class { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public string? Notes { get; set; }
        public string Itinerary { get; set; } = string.Empty;
    }
}