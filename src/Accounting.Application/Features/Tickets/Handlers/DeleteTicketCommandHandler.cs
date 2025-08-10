using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class DeleteTicketCommandHandler : ICommandHandler<DeleteTicketCommand, Result>
    {
        private readonly IAccountingDbContext _context;

        public DeleteTicketCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result.Failure<bool>("Ticket not found");
                }

                // Check if ticket can be deleted (only Unissued tickets can be deleted)
                if (ticket.Status != TicketStatus.Draft)
                {
                    return Result.Failure<bool>("Only draft tickets can be deleted");
                }

                // Remove ticket items first
                _context.TicketItems.RemoveRange(ticket.Items);
                
                // Remove the ticket
                _context.Tickets.Remove(ticket);
                
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Error deleting ticket: {ex.Message}");
            }
        }
    }
}