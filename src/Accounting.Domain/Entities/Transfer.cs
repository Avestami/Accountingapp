using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Domain.Entities
{
    public class Transfer : BaseEntity
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
        public int FromBankAccountId { get; set; }

        [Required]
        public int ToBankAccountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ExchangeRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? FeeAmount { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public TransferStatus Status { get; set; } = TransferStatus.Draft;

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("FromBankAccountId")]
        public virtual BankAccount FromBankAccount { get; set; } = null!;

        [ForeignKey("ToBankAccountId")]
        public virtual BankAccount ToBankAccount { get; set; } = null!;
    }

    public enum TransferStatus
    {
        Draft = 1,
        Posted = 2,
        Cancelled = 3
    }
}