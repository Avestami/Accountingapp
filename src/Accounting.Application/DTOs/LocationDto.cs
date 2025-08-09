using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.DTOs
{
    public class LocationDto
    {
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
        
        public string Type { get; set; } = "Location"; // Origin or Destination
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}