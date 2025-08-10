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
    public class GetIncomeByIdQueryHandler : IQueryHandler<GetIncomeByIdQuery, Result<IncomeDto>>
    {
        private readonly IAccountingDbContext _context;

        public GetIncomeByIdQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IncomeDto>> Handle(GetIncomeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _context.Incomes
                    .Include(i => i.BankAccount)
                    .Include(i => i.Counterparty)
                    .FirstOrDefaultAsync(i => i.Id == request.Id && i.Company == request.Company, cancellationToken);

                if (income == null)
                {
                    return Result.Failure<IncomeDto>("Income not found");
                }

                var incomeDto = new IncomeDto
                {
                    Id = income.Id,
                    DocumentNumber = income.DocumentNumber,
                    Date = income.Date,
                    Description = income.Description,
                    Amount = income.Amount,
                    Currency = income.Currency,
                    ExchangeRate = income.ExchangeRate,
                    LocalAmount = income.LocalAmount,
                    PaymentSource = income.PaymentSource,
                    BankAccountId = income.BankAccountId,
                    BankAccountName = income.BankAccount?.AccountName,
                    CounterpartyId = income.CounterpartyId,
                    CounterpartyName = income.Counterparty?.Name,
                    Reference = income.Reference,
                    Notes = income.Notes,
                    Status = income.Status,
                    Company = income.Company,
                    CreatedAt = income.CreatedAt,
                    UpdatedAt = income.UpdatedAt
                };

                return Result<IncomeDto>.Success(incomeDto);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<IncomeDto>($"Error retrieving income: {ex.Message}");
            }
        }
    }
}