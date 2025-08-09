using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Tickets.Commands
{
    public class DeleteTicketCommand : ICommand<Result<bool>>
    {
        public int Id { get; }

        public DeleteTicketCommand(int id)
        {
            Id = id;
        }
    }
}