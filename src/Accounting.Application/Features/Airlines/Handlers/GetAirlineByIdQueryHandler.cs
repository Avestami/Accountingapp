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
    public class GetAirlineByIdQueryHandler : IQueryHandler<GetAirlineByIdQuery, Result<AirlineDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetAirlineByIdQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AirlineDto>> Handle(GetAirlineByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var airline = await _context.Airlines
                    .FirstOrDefaultAsync(a => a.Id == request.Id && a.Company == request.Company, cancellationToken);

                if (airline == null)
                {
                    return Result.Failure<AirlineDto>("Airline not found");
                }

                var airlineDto = new AirlineDto
                {
                    Id = airline.Id,
                    Name = airline.Name,
                    Code = airline.Code,
                    Country = airline.Country,
                    IsActive = airline.IsActive,
                    CreatedAt = airline.CreatedAt,
                    UpdatedAt = airline.UpdatedAt,
                    Company = airline.Company
                };

                return Result<AirlineDto>.Success(airlineDto);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<AirlineDto>($"Error retrieving airline: {ex.Message}");
            }
        }
    }
}