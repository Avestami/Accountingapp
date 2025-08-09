using System;

namespace Accounting.Domain.Entities
{
    public class TicketItem : BaseEntity
    {
        public string PassengerName { get; set; }
        public string? PassengerAge { get; set; }
        public int AirlineId { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string FlightNumber { get; set; }
        public string SeatNumber { get; set; }
        public string Class { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public string Itinerary { get; set; }
        
        // Foreign Key
        public int TicketId { get; set; }
        
        // Navigation Properties
        public virtual Ticket Ticket { get; set; }
        public virtual Airline Airline { get; set; }
        public virtual Origin Origin { get; set; }
        public virtual Destination Destination { get; set; }
        
        public TicketItem()
        {
            Currency = "USD";
        }
    }
}