using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public int? ParentAccountId { get; set; }
        public string ParentAccountName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<AccountDto> ChildAccounts { get; set; } = new();
        public int Level { get; set; }
        public string FullPath { get; set; }
    }

    public class CreateAccountDto
    {
        [Required]
        [StringLength(20)]
        public string AccountCode { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        public int? ParentAccountId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateAccountDto
    {
        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        public int? ParentAccountId { get; set; }

        public bool IsActive { get; set; }
    }

    public class AccountSummaryDto
    {
        public int Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
        public bool HasChildren { get; set; }
    }

    public class FxTransactionDto
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; }
        public TransactionType Type { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsFullyConsumed { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int? VoucherId { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFxTransactionDto
    {
        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [StringLength(3)]
        public string FromCurrency { get; set; }

        [Required]
        [StringLength(3)]
        public string ToCurrency { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal FromAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ToAmount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int AccountId { get; set; }

        public int? VoucherId { get; set; }
    }

    public class FxConsumptionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal GainLoss { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public int BuyTransactionId { get; set; }
        public string BuyTransactionNumber { get; set; }
        public int SellTransactionId { get; set; }
        public string SellTransactionNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExchangeRateDto
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsExpired { get; set; }
    }

    public class CreateExchangeRateDto
    {
        [Required]
        [StringLength(3)]
        public string FromCurrency { get; set; }

        [Required]
        [StringLength(3)]
        public string ToCurrency { get; set; }

        [Required]
        [Range(0.000001, double.MaxValue)]
        public decimal Rate { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}