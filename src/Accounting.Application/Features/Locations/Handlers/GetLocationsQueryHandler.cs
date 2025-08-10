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
    public class GetLocationsQueryHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetLocationsQueryHandler> _logger;

        public GetLocationsQueryHandler(IAccountingDbContext context, ILogger<GetLocationsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<LocationDto>>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Locations.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.Type))
                {
                    query = query.Where(l => l.Type.ToLower() == request.Type.ToLower());
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(l => l.IsActive == request.IsActive.Value);
                }

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(l => l.Name.Contains(request.SearchTerm) || 
                                           (l.Code != null && l.Code.Contains(request.SearchTerm)));
                }

                if (request.ParentId.HasValue)
                {
                    query = query.Where(l => l.ParentId == request.ParentId.Value);
                }

                var locations = await query
                    .OrderBy(l => l.Name)
                    .Select(l => new LocationDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Type = l.Type,
                        Code = l.Code,
                        ParentId = l.ParentId,
                        IsActive = l.IsActive
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<LocationDto>>.Success(locations);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<LocationDto>>($"Error retrieving locations: {ex.Message}");
            }
        }
    }
}