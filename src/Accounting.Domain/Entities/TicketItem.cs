using System;

namespace Accounting.Domain.Entities
{
    public class TicketItem : BaseEntity
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime ItemDate { get; set; }
        public string Notes { get; set; }
        public string ReceiptPath { get; set; }
        
        // Foreign Key
        public int TicketId { get; set; }
        
        // Navigation Property
        public virtual Ticket Ticket { get; set; }
        
        public TicketItem()
        {
            Quantity = 1;
            Currency = "USD";
            ItemDate = DateTime.UtcNow;
        }
        
        public void CalculateTotal()
        {
            TotalAmount = Quantity * UnitPrice;
        }
    }
}