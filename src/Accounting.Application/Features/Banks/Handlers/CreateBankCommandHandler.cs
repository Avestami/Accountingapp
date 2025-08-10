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
    public class CreateBankCommandHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<CreateBankCommandHandler> _logger;

        public CreateBankCommandHandler(IAccountingDbContext context, ILogger<CreateBankCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<BankDto>> Handle(CreateBankCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bank = new Bank
                {
                    Name = request.Name,
                    SwiftCode = request.SwiftCode,
                    Address = request.Address,
                    Phone = request.Phone,
                    Website = request.Website,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System" // TODO: Get from current user context
                };

                _context.Banks.Add(bank);
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
                _logger.LogError(ex, "Error creating bank");
                return Result.Failure<BankDto>("An error occurred while creating the bank");
            }
        }
    }
}