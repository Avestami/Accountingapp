using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Counterparties.Commands
{
    public class CreateCounterpartyCommand
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TaxId { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsActive { get; set; } = true;

        [Range(0, double.MaxValue)]
        public decimal OpeningBalanceIRR { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OpeningBalanceUSD { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OpeningBalanceEUR { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OpeningBalanceAED { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
    }
}