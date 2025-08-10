using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Locations.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Locations.Handlers
{
    public class GetCitiesByCountryQueryHandler : IQueryHandler<GetCitiesByCountryQuery, Result<List<LocationDto>>>
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetCitiesByCountryQueryHandler> _logger;

        public GetCitiesByCountryQueryHandler(IAccountingDbContext context, ILogger<GetCitiesByCountryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<LocationDto>>> Handle(GetCitiesByCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Locations
                    .Where(l => l.Type.ToLower() == "city" && l.ParentId == request.CountryId);

                if (request.IsActive.HasValue)
                {
                    query = query.Where(l => l.IsActive == request.IsActive.Value);
                }

                var cities = await query
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

                return Result<List<LocationDto>>.Success(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cities for country {CountryId}", request.CountryId);
                return Result.Failure<List<LocationDto>>("An error occurred while retrieving cities");
            }
        }
    }
}