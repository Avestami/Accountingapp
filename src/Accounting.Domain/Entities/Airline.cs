using System.ComponentModel.DataAnnotations;

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
        public string IataCode { get; set; } = string.Empty;

        [MaxLength(4)]
        public string? IcaoCode { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        // Navigation properties
        public virtual ICollection<TicketItem> TicketItems { get; set; } = new List<TicketItem>();
    }
}