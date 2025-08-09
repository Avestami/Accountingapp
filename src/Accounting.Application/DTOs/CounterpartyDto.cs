using System;

namespace Accounting.Application.DTOs
{
    public class CounterpartyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal OpeningBalanceIRR { get; set; }
        public decimal OpeningBalanceUSD { get; set; }
        public decimal OpeningBalanceEUR { get; set; }
        public decimal OpeningBalanceAED { get; set; }
        public decimal? CreditLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        
        // Computed properties
        public decimal CurrentBalanceIRR { get; set; }
        public decimal CurrentBalanceUSD { get; set; }
        public decimal CurrentBalanceEUR { get; set; }
        public decimal CurrentBalanceAED { get; set; }
        public int TicketCount { get; set; }
        public int VoucherCount { get; set; }
    }
}