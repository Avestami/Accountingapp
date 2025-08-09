using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Counterparties.Commands
{
    public class DeleteCounterpartyCommand : ICommand<Result<bool>>
    {
        public int Id { get; }

        public DeleteCounterpartyCommand(int id)
        {
            Id = id;
        }
    }
}