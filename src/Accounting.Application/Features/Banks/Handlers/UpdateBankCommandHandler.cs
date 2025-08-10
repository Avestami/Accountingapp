using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Banks.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Banks.Handlers
{
    public class UpdateBankCommandHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<UpdateBankCommandHandler> _logger;

        public UpdateBankCommandHandler(IAccountingDbContext context, ILogger<UpdateBankCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<BankDto>> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bank = await _context.Banks.FindAsync(request.Id);
                if (bank == null)
                {
                    return Result.Failure<BankDto>("Bank not found");
                }

                bank.Name = request.Name;
                bank.SwiftCode = request.SwiftCode;
                bank.Address = request.Address;
                bank.Phone = request.Phone;
                bank.Website = request.Website;
                bank.IsActive = request.IsActive;
                bank.UpdatedAt = DateTime.UtcNow;
                bank.UpdatedBy = "System"; // TODO: Get from current user context

                await _context.SaveChangesAsync(cancellationToken);

                var bankDto = new BankDto
                {
                    Id = bank.Id,
                    Name = bank.Name,
                    SwiftCode = bank.SwiftCode,
                    Address = bank.Address,
                    Phone = bank.Phone,
                    Website = bank.Website,
                    IsActive = bank.IsActive,
                    CreatedAt = bank.CreatedAt,
                    UpdatedAt = bank.UpdatedAt
                };

                return Result<BankDto>.Success(bankDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bank with ID {BankId}", request.Id);
                return Result.Failure<BankDto>("An error occurred while updating the bank");
            }
        }
    }
}