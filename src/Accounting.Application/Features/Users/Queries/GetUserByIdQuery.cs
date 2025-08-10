using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IQuery<Result<UserDto>>
    {
        public int Id { get; set; }
    }
}