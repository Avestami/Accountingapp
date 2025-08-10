using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Locations.Queries
{
    public class GetLocationsQuery
    {
        public string? Type { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
        public int? ParentId { get; set; }
    }
}