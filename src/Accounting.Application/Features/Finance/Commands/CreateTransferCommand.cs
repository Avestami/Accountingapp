using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Finance.Commands
{
    public class CreateTransferCommand : ICommand<Result<TransferDto>>
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
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        // Custom validation method
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (FromAccountId == ToAccountId)
            {
                errorMessage = "From and To accounts cannot be the same";
                return false;
            }

            if (Date > DateTime.Now.AddDays(1))
            {
                errorMessage = "Transfer date cannot be more than 1 day in the future";
                return false;
            }

            if (Currency != "IRR" && !ExchangeRate.HasValue)
            {
                errorMessage = "Exchange rate is required for foreign currency transfers";
                return false;
            }

            return true;
        }
    }
}