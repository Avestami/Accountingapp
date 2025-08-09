using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Destination : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(10)]
        public string? TimeZone { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<TicketItem> DestinationTicketItems { get; set; } = new List<TicketItem>();
    }
}