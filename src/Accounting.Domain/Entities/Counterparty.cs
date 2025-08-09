using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Counterparty : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        public bool IsCustomer { get; set; } = false;
        public bool IsSupplier { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Opening balances in different currencies
        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalanceUSD { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalanceEUR { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalanceIRR { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalanceAED { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}