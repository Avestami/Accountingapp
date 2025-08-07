using System;
using System.Collections.Generic;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Notes { get; set; }
        public string AttachmentPath { get; set; }
        
        // Foreign Keys
        public int CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? ApprovedByUserId { get; set; }
        
        // Navigation Properties
        public virtual User CreatedByUser { get; set; }
        public virtual User AssignedToUser { get; set; }
        public virtual User ApprovedByUser { get; set; }
        public virtual ICollection<TicketItem> Items { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        
        public Ticket()
        {
            Items = new HashSet<TicketItem>();
            AuditLogs = new HashSet<AuditLog>();
            Status = TicketStatus.Draft;
            Currency = "USD";
            RequestDate = DateTime.UtcNow;
        }
        
        public bool CanBeApproved() => Status == TicketStatus.Pending;
        public bool CanBeRejected() => Status == TicketStatus.Pending;
        public bool CanBeCompleted() => Status == TicketStatus.Approved;
        public bool CanBeCancelled() => Status == TicketStatus.Draft || Status == TicketStatus.Pending;
        
        public void Approve(int approvedByUserId)
        {
            if (!CanBeApproved())
                throw new InvalidOperationException("Ticket cannot be approved in current status");
                
            Status = TicketStatus.Approved;
            ApprovalDate = DateTime.UtcNow;
            ApprovedByUserId = approvedByUserId;
        }
        
        public void Reject()
        {
            if (!CanBeRejected())
                throw new InvalidOperationException("Ticket cannot be rejected in current status");
                
            Status = TicketStatus.Rejected;
        }
        
        public void Complete()
        {
            if (!CanBeCompleted())
                throw new InvalidOperationException("Ticket cannot be completed in current status");
                
            Status = TicketStatus.Completed;
            CompletionDate = DateTime.UtcNow;
        }
        
        public void Cancel()
        {
            if (!CanBeCancelled())
                throw new InvalidOperationException("Ticket cannot be cancelled in current status");
                
            Status = TicketStatus.Cancelled;
        }
    }
}