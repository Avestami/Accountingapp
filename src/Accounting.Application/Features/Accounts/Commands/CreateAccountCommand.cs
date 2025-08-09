using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : ICommand<Result<int>>
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public AccountType Type { get; set; }
        public int? ParentAccountId { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsActive { get; set; } = true;
    }
}