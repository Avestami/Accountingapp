using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Locations.Commands;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Locations.Handlers
{
    public class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<DeleteLocationCommandHandler> _logger;

        public DeleteLocationCommandHandler(IAccountingDbContext context, ILogger<DeleteLocationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _context.Locations.FindAsync(request.Id);
                if (location == null)
                {
                    return Result.Failure<bool>("Location not found");
                }

                // Check if location has children
                var hasChildren = await _context.Locations.AnyAsync(l => l.ParentId == request.Id, cancellationToken);
                if (hasChildren)
                {
                    return Result.Failure<bool>("Cannot delete location with child locations");
                }

                // Check if location is referenced by other entities
                // Note: Counterparty entity doesn't have location references, so we'll skip this check for now
                // var isReferenced = await _context.Counterparties.AnyAsync(c => c.CountryId == request.Id || c.CityId == request.Id, cancellationToken);
                var isReferenced = false; // Placeholder until location references are added to entities

                if (isReferenced)
                {
                    return Result.Failure<bool>("Cannot delete location that is referenced by other entities");
                }

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location with ID {LocationId}", request.Id);
                return Result.Failure<bool>("An error occurred while deleting the location");
            }
        }
    }
}