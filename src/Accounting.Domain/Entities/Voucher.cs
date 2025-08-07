using System;
using System.Collections.Generic;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities
{
    public class Voucher : BaseEntity
    {
        public string VoucherNumber { get; set; }
        public VoucherType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
        public VoucherStatus Status { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedDate { get; set; }
        public int? PostedByUserId { get; set; }
        
        // Foreign Keys
        public int CreatedByUserId { get; set; }
        public int? TicketId { get; set; }
        
        // Navigation Properties
        public virtual User CreatedByUser { get; set; }
        public virtual User PostedByUser { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ICollection<VoucherEntry> Entries { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        
        public Voucher()
        {
            Entries = new HashSet<VoucherEntry>();
            AuditLogs = new HashSet<AuditLog>();
            Currency = "USD";
            VoucherDate = DateTime.UtcNow;
            Status = VoucherStatus.Draft;
            IsPosted = false;
        }
        
        public bool IsBalanced()
        {
            decimal debitTotal = 0;
            decimal creditTotal = 0;
            
            foreach (var entry in Entries)
            {
                if (entry.TransactionType == TransactionType.Debit)
                    debitTotal += entry.Amount;
                else
                    creditTotal += entry.Amount;
            }
            
            return Math.Abs(debitTotal - creditTotal) < 0.01m;
        }
        
        public void Post(int postedByUserId)
        {
            if (IsPosted)
                throw new InvalidOperationException("Voucher is already posted");
                
            if (!IsBalanced())
                throw new InvalidOperationException("Voucher entries are not balanced");
                
            if (Status != VoucherStatus.Approved)
                throw new InvalidOperationException("Only approved vouchers can be posted");
                
            IsPosted = true;
            PostedDate = DateTime.UtcNow;
            PostedByUserId = postedByUserId;
            Status = VoucherStatus.Posted;
        }
        
        public void Unpost()
        {
            if (!IsPosted)
                throw new InvalidOperationException("Voucher is not posted");
                
            IsPosted = false;
            PostedDate = null;
            PostedByUserId = null;
            Status = VoucherStatus.Approved;
        }
        
        public void Approve()
        {
            if (Status != VoucherStatus.Pending)
                throw new InvalidOperationException("Only pending vouchers can be approved");
                
            Status = VoucherStatus.Approved;
        }
        
        public void Reject()
        {
            if (Status != VoucherStatus.Pending)
                throw new InvalidOperationException("Only pending vouchers can be rejected");
                
            Status = VoucherStatus.Rejected;
        }
        
        public void Submit()
        {
            if (Status != VoucherStatus.Draft)
                throw new InvalidOperationException("Only draft vouchers can be submitted");
                
            if (!IsBalanced())
                throw new InvalidOperationException("Voucher entries are not balanced");
                
            Status = VoucherStatus.Pending;
        }
        
        public void Cancel()
        {
            if (IsPosted)
                throw new InvalidOperationException("Posted vouchers cannot be cancelled");
                
            Status = VoucherStatus.Cancelled;
        }
    }
}