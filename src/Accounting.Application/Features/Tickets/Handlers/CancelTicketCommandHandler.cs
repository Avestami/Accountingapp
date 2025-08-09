using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Tickets.Handlers
{
    public class CancelTicketCommandHandler : ICommandHandler<CancelTicketCommand, Result>
    {
        private readonly IAccountingDbContext _context;

        public CancelTicketCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(CancelTicketCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

                if (ticket == null)
                {
                    return Result.Failure("Ticket not found");
                }

                // Check if ticket can be cancelled (only non-cancelled tickets can be cancelled)
                if (ticket.Status == TicketStatus.Cancelled)
                {
                    return Result.Failure("Ticket is already cancelled");
                }

                // Update ticket status
                ticket.Status = TicketStatus.Cancelled;
                ticket.CancellationReason = command.Reason;
                ticket.UpdatedAt = DateTime.UtcNow;

                // Add notes if provided
                if (!string.IsNullOrEmpty(command.Notes))
                {
                    ticket.Description = string.IsNullOrEmpty(ticket.Description) 
                        ? $"Cancellation Notes: {command.Notes}"
                        : $"{ticket.Description}\nCancellation Notes: {command.Notes}";
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error cancelling ticket: {ex.Message}");
            }
        }
    }
}