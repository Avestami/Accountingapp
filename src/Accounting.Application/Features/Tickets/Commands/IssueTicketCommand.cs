using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Tickets.Commands
{
    public class IssueTicketCommand : ICommand<Result<TicketDto>>
    {
        public int Id { get; set; }
        public string? Notes { get; }
        public string? IssuedBy { get; set; }

        public IssueTicketCommand(int id, string? notes = null)
        {
            Id = id;
            Notes = notes;
        }
    }
}