using System;
using Accounting.Domain.Entities;

namespace Accounting.Application.DTOs
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IRR";
        public decimal? ExchangeRate { get; set; }
        public decimal LocalAmount { get; set; }
        public PaymentSource PaymentSource { get; set; }
        public int? BankAccountId { get; set; }
        public string? BankAccountName { get; set; }
        public int? CounterpartyId { get; set; }
        public string? CounterpartyName { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public IncomeStatus Status { get; set; }
        public string Company { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}