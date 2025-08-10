using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class BankAccount : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BankId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IRR";

        [Column(TypeName = "decimal(18,4)")]
        public decimal OpeningBalance { get; set; } = 0;

        [MaxLength(50)]
        public string? Iban { get; set; }

        [MaxLength(20)]
        public string? SortCode { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("BankId")]
        public virtual Bank Bank { get; set; } = null!;

        public virtual ICollection<Voucher> VouchersAsPayerAccount { get; set; } = new List<Voucher>();
        public virtual ICollection<Voucher> VouchersAsOurBankAccount { get; set; } = new List<Voucher>();
    }
}