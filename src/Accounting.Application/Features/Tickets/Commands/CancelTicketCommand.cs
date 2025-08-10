using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Tickets.Commands
{
    public class CancelTicketCommand : ICommand<Result<TicketDto>>
    {
        public int Id { get; }
        public string Reason { get; }
        public string? Notes { get; }
        public string? CancelledBy { get; set; }

        public CancelTicketCommand(int id, string reason, string? notes = null)
        {
            Id = id;
            Reason = reason;
            Notes = notes;
        }
    }
}