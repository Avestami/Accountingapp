using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Airlines.Commands;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Airlines.Handlers
{
    public class DeleteAirlineCommandHandler : ICommandHandler<DeleteAirlineCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public DeleteAirlineCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteAirlineCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var airline = await _context.Airlines
                    .FirstOrDefaultAsync(a => a.Id == command.Id && a.Company == command.Company, cancellationToken);

                if (airline == null)
                {
                    return Result.Failure<bool>("Airline not found");
                }

                // Check if airline is being used in any tickets
                var hasTickets = await _context.TicketItems
                    .AnyAsync(t => t.AirlineId == command.Id, cancellationToken);

                if (hasTickets)
                {
                    return Result.Failure<bool>("Cannot delete airline as it is being used in tickets");
                }

                _context.Airlines.Remove(airline);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Error deleting airline: {ex.Message}");
            }
        }
    }
}