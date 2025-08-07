using System;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class FxTransaction : BaseEntity
    {
        public string TransactionNumber { get; set; }
        public FxTransactionType Type { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
        
        // FIFO Tracking
        public decimal RemainingAmount { get; set; }
        public bool IsFullyConsumed { get; set; }
        
        // Foreign Keys
        public int AccountId { get; set; }
        public int? VoucherId { get; set; }
        
        // Navigation Properties
        public virtual Account Account { get; set; }
        public virtual Voucher Voucher { get; set; }
        public virtual ICollection<FxConsumption> Consumptions { get; set; }
        public virtual ICollection<FxConsumption> ConsumedFrom { get; set; }
        
        public FxTransaction()
        {
            Consumptions = new HashSet<FxConsumption>();
            ConsumedFrom = new HashSet<FxConsumption>();
            TransactionDate = DateTime.UtcNow;
            IsFullyConsumed = false;
        }
        
        public void Initialize()
        {
            if (Type == FxTransactionType.Buy)
            {
                RemainingAmount = ToAmount; // Amount in foreign currency
            }
            else if (Type == FxTransactionType.Sell)
            {
                RemainingAmount = 0; // Sell transactions don't have remaining amounts
                IsFullyConsumed = true;
            }
        }
        
        public bool CanConsume(decimal amount)
        {
            return Type == FxTransactionType.Buy && RemainingAmount >= amount && !IsFullyConsumed;
        }
        
        public void Consume(decimal amount)
        {
            if (!CanConsume(amount))
                throw new InvalidOperationException("Cannot consume the specified amount from this transaction");
                
            RemainingAmount -= amount;
            if (RemainingAmount <= 0.01m)
            {
                RemainingAmount = 0;
                IsFullyConsumed = true;
            }
        }
    }
    
    public enum FxTransactionType
    {
        Buy = 1,
        Sell = 2
    }
}