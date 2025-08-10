using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Locations.Commands
{
    public class CreateLocationCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}