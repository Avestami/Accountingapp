using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Airlines.Commands
{
    public class UpdateAirlineCommand : ICommand<Result<AirlineDto>>
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
        
        public string Company { get; set; } = string.Empty;
    }
}