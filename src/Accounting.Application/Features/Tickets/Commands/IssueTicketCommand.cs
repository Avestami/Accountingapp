namespace Accounting.Application.Features.Tickets.Commands
{
    public class IssueTicketCommand
    {
        public int Id { get; }
        public string? Notes { get; }

        public IssueTicketCommand(int id, string? notes = null)
        {
            Id = id;
            Notes = notes;
        }
    }
}