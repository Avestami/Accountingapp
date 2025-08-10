using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Locations.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Locations.Handlers
{
    public class GetLocationByIdQueryHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetLocationByIdQueryHandler> _logger;

        public GetLocationByIdQueryHandler(IAccountingDbContext context, ILogger<GetLocationByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<LocationDto>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _context.Locations
                    .Where(l => l.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (location == null)
                {
                    return Result.Failure<LocationDto>("Location not found");
                }

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Code = location.Code,
                    IsActive = location.IsActive,
                    Type = location.Type,
                    ParentId = location.ParentId,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                    // Note: Parent and Children properties are not available in LocationDto
                    // Parent = location.Parent != null ? new LocationDto { ... } : null,
                    // Children = location.Children?.Select(c => new LocationDto { ... }).ToList()
                };

                return Result<LocationDto>.Success(locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location with ID {LocationId}", request.Id);
                return Result.Failure<LocationDto>("An error occurred while retrieving the location");
            }
        }
    }
}