using MediatR;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Airlines.Queries
{
    public class GetAirlinesQuery : IQuery<Result<PagedResult<AirlineDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}