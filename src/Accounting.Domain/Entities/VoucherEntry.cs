using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities
{
    public class VoucherEntry : BaseEntity
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Currency { get; set; }
        
        // Foreign Keys
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        
        // Navigation Properties
        public virtual Voucher Voucher { get; set; }
        public virtual Account Account { get; set; }
        
        public VoucherEntry()
        {
            Currency = "USD";
        }
    }
}