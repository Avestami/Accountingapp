using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Finance.Commands
{
    public class CreateCostCommand : IRequest<Result<CostDto>>
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

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
    }
}