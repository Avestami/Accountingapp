using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Domain.Enums;
using Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class DeleteTicketCommandHandler : ICommandHandler<DeleteTicketCommand, Result<bool>>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result<bool>.Failure("Ticket not found");
                }

                // Check if ticket can be deleted (only Unissued tickets can be deleted)
                if (ticket.Status != TicketStatus.Unissued)
                {
                    return Result<bool>.Failure("Only unissued tickets can be deleted");
                }

                // Remove ticket items first
                _context.TicketItems.RemoveRange(ticket.Items);
                
                // Remove the ticket
                _context.Tickets.Remove(ticket);
                
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting ticket: {ex.Message}");
            }
        }
    }
}