namespace Accounting.Application.Features.Tickets.Commands
{
    public class DeleteTicketCommand
    {
        public int Id { get; }

        public DeleteTicketCommand(int id)
        {
            Id = id;
        }
    }
}