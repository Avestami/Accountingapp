using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Airline : BaseEntity
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
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public string Company { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<TicketItem> TicketItems { get; set; } = new List<TicketItem>();
    }
}