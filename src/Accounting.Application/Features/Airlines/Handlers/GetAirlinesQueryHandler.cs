using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Airlines.Queries;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Airlines.Handlers
{
    public class GetAirlinesQueryHandler : IQueryHandler<GetAirlinesQuery, Result<PagedResult<AirlineDto>>>
    {
        private readonly IAccountingDbContext _context;

        public GetAirlinesQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<AirlineDto>>> Handle(GetAirlinesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Airlines
                    .Where(a => a.Company == request.Company)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                    query = query.Where(a => a.Name.Contains(request.SearchTerm) || 
                                           a.Code.Contains(request.SearchTerm) ||
                                           (a.Country != null && a.Country.Contains(request.SearchTerm)));

                if (request.IsActive.HasValue)
                    query = query.Where(a => a.IsActive == request.IsActive.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var airlines = await query
                    .OrderBy(a => a.Name)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var airlineDtos = airlines.Select(a => new AirlineDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Country = a.Country,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    Company = a.Company
                }).ToList();

                var result = new PagedResult<AirlineDto>(
                    airlineDtos,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                return Result.Success(result);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<PagedResult<AirlineDto>>($"Error retrieving airlines: {ex.Message}");
            }
        }
    }
}