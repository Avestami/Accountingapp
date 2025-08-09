using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Accounts.Commands
{
    public class DeleteAccountCommand : ICommand<Result<bool>>
    {
        public int Id { get; set; }
    }
}