using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Counterparties.Queries
{
    public class GetCounterpartiesQuery : IQuery<Result<PagedResult<CounterpartyDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsCustomer { get; set; }
        public bool? IsSupplier { get; set; }
        public bool? IsActive { get; set; }
        public string? Currency { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }
        public string SortBy { get; set; } = "Name";
        public string SortDirection { get; set; } = "asc";
    }
}