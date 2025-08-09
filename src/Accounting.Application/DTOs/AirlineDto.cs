using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.DTOs
{
    public class AirlineDto
    {
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
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}