using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Queries;
using Accounting.Application.Interfaces;
using AutoMapper;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class GetIncomesQueryHandler : IRequestHandler<GetIncomesQuery, Result<PagedResult<IncomeDto>>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;

        public GetIncomesQueryHandler(IAccountingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<IncomeDto>>> Handle(GetIncomesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Incomes
                    .Where(i => i.Company == request.Company)
                    .AsQueryable();

                // Apply filters
                if (request.FromDate.HasValue)
                    query = query.Where(i => i.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(i => i.Date <= request.ToDate.Value);

                if (!string.IsNullOrEmpty(request.Currency))
                    query = query.Where(i => i.Currency == request.Currency);

                if (request.CounterpartyId.HasValue)
                    query = query.Where(i => i.CounterpartyId == request.CounterpartyId.Value);

                if (!string.IsNullOrEmpty(request.SearchTerm))
                    query = query.Where(i => i.Description.Contains(request.SearchTerm) || 
                                           i.Reference.Contains(request.SearchTerm));

                var totalCount = await query.CountAsync(cancellationToken);

                var incomes = await query
                    .Include(i => i.BankAccount)
                    .Include(i => i.Counterparty)
                    .OrderByDescending(i => i.Date)
                    .ThenByDescending(i => i.Id)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var incomeDtos = incomes.Select(i => new IncomeDto
                {
                    Id = i.Id,
                    DocumentNumber = i.DocumentNumber,
                    Date = i.Date,
                    Description = i.Description,
                    Amount = i.Amount,
                    Currency = i.Currency,
                    ExchangeRate = i.ExchangeRate,
                    LocalAmount = i.LocalAmount,
                    PaymentSource = i.PaymentSource,
                    BankAccountId = i.BankAccountId,
                    BankAccountName = i.BankAccount?.AccountName,
                    CounterpartyId = i.CounterpartyId,
                    CounterpartyName = i.Counterparty?.Name,
                    Reference = i.Reference,
                    Notes = i.Notes,
                    Status = i.Status,
                    Company = i.Company,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                }).ToList();

                var result = new PagedResult<IncomeDto>(
                    incomeDtos,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                return Result.Success(result);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<PagedResult<IncomeDto>>($"Error retrieving incomes: {ex.Message}");
            }
        }
    }
}