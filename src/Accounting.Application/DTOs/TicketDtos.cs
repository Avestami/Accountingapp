using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Notes { get; set; }
        public string RejectionReason { get; set; }
        public int RequestedByUserId { get; set; }
        public string RequestedByUserName { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public int? ProcessedByUserId { get; set; }
        public string ProcessedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TicketItemDto> Items { get; set; } = new();
    }

    public class CreateTicketDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [StringLength(500)]
        public string Notes { get; set; }

        public List<CreateTicketItemDto> Items { get; set; } = new();
    }

    public class UpdateTicketDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public List<UpdateTicketItemDto> Items { get; set; } = new();
    }

    public class ApproveTicketDto
    {
        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class RejectTicketDto
    {
        [Required]
        [StringLength(500)]
        public string RejectionReason { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class ProcessTicketDto
    {
        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class TicketItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime ItemDate { get; set; }
        public string Notes { get; set; }
        public string ReceiptPath { get; set; }
        public int TicketId { get; set; }
    }

    public class CreateTicketItemDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [Required]
        public DateTime ItemDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [StringLength(500)]
        public string ReceiptPath { get; set; }
    }

    public class UpdateTicketItemDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [Required]
        public DateTime ItemDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [StringLength(500)]
        public string ReceiptPath { get; set; }
    }

    public class TicketSummaryDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestedByUserName { get; set; }
        public int ItemsCount { get; set; }
    }
}