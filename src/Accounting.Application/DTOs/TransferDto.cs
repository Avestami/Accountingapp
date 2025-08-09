using System;
using Accounting.Domain.Entities;

namespace Accounting.Application.DTOs
{
    public class TransferDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public decimal? ExchangeRate { get; set; }
        public decimal LocalAmount { get; set; }
        public int FromAccountId { get; set; }
        public string FromAccountName { get; set; } = string.Empty;
        public int ToAccountId { get; set; }
        public string ToAccountName { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public TransferStatus Status { get; set; }
        public string Company { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}