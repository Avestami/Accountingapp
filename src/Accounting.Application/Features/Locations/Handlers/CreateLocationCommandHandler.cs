using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Locations.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Locations.Handlers
{
    public class CreateLocationCommandHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<CreateLocationCommandHandler> _logger;

        public CreateLocationCommandHandler(IAccountingDbContext context, ILogger<CreateLocationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<LocationDto>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate location type
                var validTypes = new[] { "Country", "City", "Region", "State" };
                if (!Array.Exists(validTypes, t => t.Equals(request.Type, StringComparison.OrdinalIgnoreCase)))
                {
                    return Result.Failure<LocationDto>("Invalid location type. Valid types are: Country, City, Region, State");
                }

                // Validate parent relationship
                if (request.ParentId.HasValue)
                {
                    var parent = await _context.Locations.FindAsync(request.ParentId.Value);
                    if (parent == null)
                    {
                        return Result.Failure<LocationDto>("Parent location not found");
                    }

                    // Validate parent-child relationship rules
                    if (request.Type.Equals("Country", StringComparison.OrdinalIgnoreCase) && request.ParentId.HasValue)
                    {
                        return Result.Failure<LocationDto>("Country cannot have a parent location");
                    }

                    if (request.Type.Equals("City", StringComparison.OrdinalIgnoreCase) && 
                        !parent.Type.Equals("Country", StringComparison.OrdinalIgnoreCase))
                    {
                        return Result.Failure<LocationDto>("City must have a Country as parent");
                    }
                }

                var location = new Location
                {
                    Name = request.Name,
                    Type = request.Type,
                    Code = request.Code,
                    ParentId = request.ParentId,
                    IsActive = request.IsActive
                };

                _context.Locations.Add(location);
                await _context.SaveChangesAsync(cancellationToken);

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Type = location.Type,
                    Code = location.Code,
                    ParentId = location.ParentId,
                    IsActive = location.IsActive
                };

                return Result<LocationDto>.Success(locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location");
                return Result.Failure<LocationDto>("An error occurred while creating the location");
            }
        }
    }
}