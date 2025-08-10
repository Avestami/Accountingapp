using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Finance.Commands
{
    public class CreateIncomeCommand : ICommand<Result<IncomeDto>>
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a valid 3-letter code")]
        public string Currency { get; set; } = "IRR";

        [Range(0.01, double.MaxValue, ErrorMessage = "Exchange rate must be greater than 0")]
        public decimal? ExchangeRate { get; set; }

        [Required]
        public PaymentSource PaymentSource { get; set; }

        public int? BankAccountId { get; set; }
        public int? CounterpartyId { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if date is not too far in the future (more than 1 year)
            if (Date > DateTime.Now.AddYears(1))
            {
                errorMessage = "Date cannot be more than 1 year in the future";
                return false;
            }

            // Check if exchange rate is required for foreign currency
            if (Currency != "IRR" && (!ExchangeRate.HasValue || ExchangeRate.Value <= 0))
            {
                errorMessage = "Exchange rate is required for foreign currency transactions";
                return false;
            }

            // Check if bank account is required for bank payment source
            if (PaymentSource == PaymentSource.Bank && !BankAccountId.HasValue)
            {
                errorMessage = "Bank account is required for bank payment source";
                return false;
            }

            return true;
        }
    }
}