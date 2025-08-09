using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Queries;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class GetTransferByIdQueryHandler : IQueryHandler<GetTransferByIdQuery, Result<TransferDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetTransferByIdQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TransferDto>> Handle(GetTransferByIdQuery request, CancellationToken cancellationToken)
        {
            var transfer = await _context.Transfers
                .Include(t => t.FromBankAccount)
                .Include(t => t.ToBankAccount)
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.Company == request.Company, cancellationToken);

            if (transfer == null)
            {
                return Result.Failure<TransferDto>("Transfer not found");
            }

            var transferDto = new TransferDto
            {
                Id = transfer.Id,
                DocumentNumber = transfer.DocumentNumber,
                Date = transfer.Date,
                Description = transfer.Description,
                Amount = transfer.Amount,
                Currency = transfer.Currency,
                ExchangeRate = transfer.ExchangeRate,
                LocalAmount = transfer.ExchangeRate.HasValue ? transfer.Amount * transfer.ExchangeRate.Value : transfer.Amount,
                FromAccountId = transfer.FromBankAccountId,
                FromAccountName = transfer.FromBankAccount?.AccountName ?? string.Empty,
                ToAccountId = transfer.ToBankAccountId,
                ToAccountName = transfer.ToBankAccount?.AccountName ?? string.Empty,
                Reference = transfer.Reference,
                Notes = transfer.Notes,
                Status = transfer.Status,
                Company = transfer.Company,
                CreatedAt = transfer.CreatedAt,
                UpdatedAt = transfer.UpdatedAt
            };

            return Result.Success(transferDto);
        }
    }
}