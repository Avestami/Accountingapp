using System.Linq;
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
    public class GetCostByIdQueryHandler : IQueryHandler<GetCostByIdQuery, Result<CostDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetCostByIdQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CostDto>> Handle(GetCostByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cost = await _context.Costs
                    .Include(c => c.BankAccount)
                    .Include(c => c.Counterparty)
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.Company == request.Company, cancellationToken);

                if (cost == null)
                {
                    return Result.Failure<CostDto>("Cost not found");
                }

                var costDto = new CostDto
                {
                    Id = cost.Id,
                    DocumentNumber = cost.DocumentNumber,
                    Date = cost.Date,
                    Description = cost.Description,
                    Amount = cost.Amount,
                    Currency = cost.Currency,
                    ExchangeRate = cost.ExchangeRate,
                    LocalAmount = cost.LocalAmount,
                    PaymentSource = cost.PaymentSource,
                    BankAccountId = cost.BankAccountId,
                    BankAccountName = cost.BankAccount?.AccountName,
                    CounterpartyId = cost.CounterpartyId,
                    CounterpartyName = cost.Counterparty?.Name,
                    Reference = cost.Reference,
                    Notes = cost.Notes,
                    Status = cost.Status,
                    Company = cost.Company,
                    CreatedAt = cost.CreatedAt,
                    UpdatedAt = cost.UpdatedAt
                };

                return Result.Success(costDto);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<CostDto>($"Error retrieving cost: {ex.Message}");
            }
        }
    }
}