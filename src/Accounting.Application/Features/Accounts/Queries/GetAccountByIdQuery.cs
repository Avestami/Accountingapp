using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Accounts.Queries
{
    public class GetAccountByIdQuery : IQuery<Result<AccountDto>>
    {
        public int Id { get; set; }
    }
}