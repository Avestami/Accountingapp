using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Users.Commands
{
    public class DeleteUserCommand : ICommand<Result<bool>>
    {
        public int Id { get; set; }
    }
}