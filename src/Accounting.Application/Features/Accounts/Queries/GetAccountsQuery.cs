using System.Collections.Generic;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Accounts.Queries
{
    public class GetAccountsQuery : IQuery<Result<List<AccountDto>>>
    {
        public bool? IsActive { get; set; }
        public int? ParentAccountId { get; set; }
        public string AccountType { get; set; }
    }
}