using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Airlines.Commands;
using Accounting.Domain.Entities;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Airlines.Handlers
{
    public class CreateAirlineCommandHandler : ICommandHandler<CreateAirlineCommand, Result<AirlineDto>>
    {
        private readonly IAccountingDbContext _context;

        public CreateAirlineCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AirlineDto>> Handle(CreateAirlineCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Check if code already exists
                var existingAirline = await _context.Airlines
                    .FirstOrDefaultAsync(a => a.Code == command.Code && a.Company == command.Company, cancellationToken);

                if (existingAirline != null)
                {
                    return Result.Failure<AirlineDto>("Airline code already exists");
                }

                var airline = new Airline
                {
                    Name = command.Name,
                    Code = command.Code,
                    Country = command.Country,
                    IsActive = command.IsActive,
                    Company = command.Company,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Airlines.Add(airline);
                await _context.SaveChangesAsync(cancellationToken);

                var dto = MapToDto(airline);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure<AirlineDto>($"Error creating airline: {ex.Message}");
            }
        }

        private AirlineDto MapToDto(Airline airline)
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