using System;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public AuditAction Action { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string Changes { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalInfo { get; set; }
        
        // Foreign Keys
        public int UserId { get; set; }
        public int? TicketId { get; set; }
        public int? VoucherId { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Voucher Voucher { get; set; }
        
        public AuditLog()
        {
            Timestamp = DateTime.UtcNow;
        }
        
        public static AuditLog Create(string entityName, string entityId, AuditAction action, 
            int userId, string oldValues = null, string newValues = null, string changes = null)
        {
            return new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                OldValues = oldValues,
                NewValues = newValues,
                Changes = changes
            };
        }
        
        public void SetHttpContext(string ipAddress, string userAgent)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
        
        public void AddAdditionalInfo(string info)
        {
            if (string.IsNullOrEmpty(AdditionalInfo))
                AdditionalInfo = info;
            else
                AdditionalInfo += $"; {info}";
        }
    }
}