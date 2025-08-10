using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Users.Queries
{
    public class GetUsersQuery : IQuery<Result<PagedResult<UserDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}