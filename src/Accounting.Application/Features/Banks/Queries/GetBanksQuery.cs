using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Banks.Queries
{
    public class GetBanksQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "Name";
        public string SortDirection { get; set; } = "asc";
    }
}