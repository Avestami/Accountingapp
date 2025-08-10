using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Banks.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Banks.Handlers
{
    public class GetBankByIdQueryHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetBankByIdQueryHandler> _logger;

        public GetBankByIdQueryHandler(IAccountingDbContext context, ILogger<GetBankByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<BankDto>> Handle(GetBankByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bank = await _context.Banks
                    .Where(b => b.Id == request.Id)
                    .Select(b => new BankDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        SwiftCode = b.SwiftCode,
                        Address = b.Address,
                        Phone = b.Phone,
                        Website = b.Website,
                        IsActive = b.IsActive,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (bank == null)
                {
                    return Result.Failure<BankDto>("Bank not found");
                }

                return Result<BankDto>.Success(bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bank with ID {BankId}", request.Id);
                return Result.Failure<BankDto>("An error occurred while retrieving the bank");
            }
        }
    }
}