using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Airlines.Commands;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Airlines.Handlers
{
    public class UpdateAirlineCommandHandler : ICommandHandler<UpdateAirlineCommand, Result<AirlineDto>>
    {
        private readonly IAccountingDbContext _context;

        public UpdateAirlineCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AirlineDto>> Handle(UpdateAirlineCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var airline = await _context.Airlines
                    .FirstOrDefaultAsync(a => a.Id == command.Id && a.Company == command.Company, cancellationToken);

                if (airline == null)
                {
                    return Result.Failure<AirlineDto>("Airline not found");
                }

                // Check if code already exists for another airline
                var existingAirline = await _context.Airlines
                    .FirstOrDefaultAsync(a => a.Code == command.Code && a.Id != command.Id && a.Company == command.Company, cancellationToken);

                if (existingAirline != null)
                {
                    return Result.Failure<AirlineDto>("Airline code already exists");
                }

                airline.Name = command.Name;
                airline.Code = command.Code;
                airline.Country = command.Country;
                airline.IsActive = command.IsActive;
                airline.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                var dto = MapToDto(airline);
                return Result<AirlineDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure<AirlineDto>($"Error updating airline: {ex.Message}");
            }
        }

        private AirlineDto MapToDto(Domain.Entities.Airline airline)
        {
            return new AirlineDto
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
        }
    }
}