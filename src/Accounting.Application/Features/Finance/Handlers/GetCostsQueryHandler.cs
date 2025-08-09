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
    public class GetCostsQueryHandler : IRequestHandler<GetCostsQuery, Result<PagedResult<CostDto>>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;

        public GetCostsQueryHandler(IAccountingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<CostDto>>> Handle(GetCostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Costs
                    .Where(c => c.Company == request.Company)
                    .AsQueryable();

                // Apply filters
                if (request.FromDate.HasValue)
                    query = query.Where(c => c.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(c => c.Date <= request.ToDate.Value);

                if (!string.IsNullOrEmpty(request.Currency))
                    query = query.Where(c => c.Currency == request.Currency);

                if (request.CounterpartyId.HasValue)
                    query = query.Where(c => c.CounterpartyId == request.CounterpartyId.Value);

                if (!string.IsNullOrEmpty(request.SearchTerm))
                    query = query.Where(c => c.Description.Contains(request.SearchTerm) || 
                                           c.Reference.Contains(request.SearchTerm));

                var totalCount = await query.CountAsync(cancellationToken);

                var costs = await query
                    .Include(c => c.BankAccount)
                    .Include(c => c.Counterparty)
                    .OrderByDescending(c => c.Date)
                    .ThenByDescending(c => c.Id)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var costDtos = costs.Select(c => new CostDto
                {
                    Id = c.Id,
                    DocumentNumber = c.DocumentNumber,
                    Date = c.Date,
                    Description = c.Description,
                    Amount = c.Amount,
                    Currency = c.Currency,
                    ExchangeRate = c.ExchangeRate,
                    LocalAmount = c.LocalAmount,
                    PaymentSource = c.PaymentSource,
                    BankAccountId = c.BankAccountId,
                    BankAccountName = c.BankAccount?.AccountName,
                    CounterpartyId = c.CounterpartyId,
                    CounterpartyName = c.Counterparty?.Name,
                    Reference = c.Reference,
                    Notes = c.Notes,
                    Status = c.Status,
                    Company = c.Company,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                var result = new PagedResult<CostDto>(
                    costDtos,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                return Result.Success(result);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<PagedResult<CostDto>>($"Error retrieving costs: {ex.Message}");
            }
        }
    }
}