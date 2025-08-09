using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Domain.Entities
{
    public class LedgerEntry : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        public int? DocumentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,4)")]
        public decimal DebitAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,4)")]
        public decimal CreditAmount { get; set; } = 0;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ExchangeRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal LocalDebitAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,4)")]
        public decimal LocalCreditAmount { get; set; } = 0;

        public int? CounterpartyId { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("CounterpartyId")]
        public virtual Counterparty? Counterparty { get; set; }
    }
}