using System;
using System.Collections.Generic;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string ProfilePicture { get; set; }
        
        // Navigation properties
        public virtual ICollection<Ticket> CreatedTickets { get; set; }
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        public virtual ICollection<Voucher> CreatedVouchers { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        
        public User()
        {
            CreatedTickets = new HashSet<Ticket>();
            AssignedTickets = new HashSet<Ticket>();
            CreatedVouchers = new HashSet<Voucher>();
            AuditLogs = new HashSet<AuditLog>();
            IsActive = true;
        }
        
        public string GetFullName() => $"{FirstName} {LastName}".Trim();
    }
}