using System;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public AuditAction Action { get; set; }
        public string ActionName => Action.ToString();
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string Changes { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalInfo { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int? TicketId { get; set; }
        public int? VoucherId { get; set; }
    }
}