using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Tickets.Commands
{
    public class CreateTicketCommand : ICommand<Result<TicketDto>>
    {
        [Required]
        public int CounterpartyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        public TicketType Type { get; set; } = TicketType.Travel;

        public List<CreateTicketItemDto> Items { get; set; } = new List<CreateTicketItemDto>();
    }

    public class CreateTicketItemDto
    {
        [Required]
        [MaxLength(100)]
        public string PassengerName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PassengerAge { get; set; }

        public int? AirlineId { get; set; }
        public int? OriginId { get; set; }
        public int? DestinationId { get; set; }

        public DateTime? ServiceDate { get; set; }

        [MaxLength(20)]
        public string? FlightNumber { get; set; }

        [MaxLength(10)]
        public string? SeatNumber { get; set; }

        [MaxLength(10)]
        public string? Class { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}