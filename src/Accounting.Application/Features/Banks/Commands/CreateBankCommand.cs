using System.ComponentModel.DataAnnotations;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Banks.Commands
{
    public class CreateBankCommand
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? SwiftCode { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        public bool IsActive { get; set; } = true;
    }
}