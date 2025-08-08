namespace Accounting.Application.Features.Tickets.Commands
{
    public class CancelTicketCommand
    {
        public int Id { get; }
        public string Reason { get; }
        public string? Notes { get; }

        public CancelTicketCommand(int id, string reason, string? notes = null)
        {
            Id = id;
            Reason = reason;
            Notes = notes;
        }
    }
}