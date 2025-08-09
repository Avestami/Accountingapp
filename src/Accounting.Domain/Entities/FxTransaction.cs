using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Domain.Entities
{
    [Table("FxTransactions")]
    public class FxTransaction : BaseEntity
    {
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal RemainingAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal ExchangeRate { get; set; }

        [Required]
        public FxTransactionType TransactionType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<FxConsumption> Consumptions { get; set; }
        
        public FxTransaction()
        {
            Consumptions = new HashSet<FxConsumption>();
            Date = DateTime.UtcNow;
        }
        
        public bool CanConsume(decimal amount)
        {
            return TransactionType == FxTransactionType.Purchase && RemainingAmount >= amount;
        }
        
        public void Consume(decimal amount)
        {
            if (!CanConsume(amount))
                throw new InvalidOperationException("Cannot consume the specified amount from this transaction");
                
            RemainingAmount -= amount;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public enum FxTransactionType
    {
        Purchase = 1,
        Sale = 2
    }
}