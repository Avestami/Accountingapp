namespace Accounting.Application.Features.Counterparties.Commands
{
    public class DeleteCounterpartyCommand
    {
        public int Id { get; }

        public DeleteCounterpartyCommand(int id)
        {
            Id = id;
        }
    }
}