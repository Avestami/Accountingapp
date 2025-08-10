using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Location : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty; // country, city, airport

        [MaxLength(10)]
        public string? Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Location? Parent { get; set; }
        public virtual ICollection<Location> Children { get; set; } = new List<Location>();

        // Computed property for frontend
        public string? ParentName => Parent?.Name;
    }
}