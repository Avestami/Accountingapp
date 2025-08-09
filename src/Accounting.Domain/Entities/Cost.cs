using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Domain.Entities
{
    public class Cost : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ExchangeRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal LocalAmount { get; set; }

        [Required]
        public PaymentSource PaymentSource { get; set; }

        public int? BankAccountId { get; set; }
        public int? CounterpartyId { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public CostStatus Status { get; set; } = CostStatus.Draft;

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("BankAccountId")]
        public virtual BankAccount? BankAccount { get; set; }

        [ForeignKey("CounterpartyId")]
        public virtual Counterparty? Counterparty { get; set; }
    }

    public enum PaymentSource
    {
        Cash = 1,
        Bank = 2,
        CounterpartyCredit = 3
    }

    public enum CostStatus
    {
        Draft = 1,
        Posted = 2,
        Cancelled = 3
    }
}