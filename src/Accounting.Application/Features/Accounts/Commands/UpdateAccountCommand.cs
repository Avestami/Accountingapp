using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Accounts.Commands
{
    public class UpdateAccountCommand : ICommand<Result<bool>>
    {
        public int Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public AccountType Type { get; set; }
        public int? ParentAccountId { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
    }
}