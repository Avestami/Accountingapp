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
    public class GetCountriesQueryHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetCountriesQueryHandler> _logger;

        public GetCountriesQueryHandler(IAccountingDbContext context, ILogger<GetCountriesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<LocationDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Locations
                    .Where(l => l.Type.ToLower() == "country");

                if (request.IsActive.HasValue)
                {
                    query = query.Where(l => l.IsActive == request.IsActive.Value);
                }

                var countries = await query
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

                return Result<List<LocationDto>>.Success(countries);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<LocationDto>>($"Error retrieving countries: {ex.Message}");
            }
        }
    }
}