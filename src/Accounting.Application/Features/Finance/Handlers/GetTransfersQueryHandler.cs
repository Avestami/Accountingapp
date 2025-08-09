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
    public class GetTransfersQueryHandler : IQueryHandler<GetTransfersQuery, Result<PagedResult<TransferDto>>>
    {
        private readonly IAccountingDbContext _context;

        public GetTransfersQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<TransferDto>>> Handle(GetTransfersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Transfers
                    .Include(t => t.FromBankAccount)
                    .Include(t => t.ToBankAccount)
                    .Where(t => t.Company == request.Company);

                // Apply filters
                if (request.FromDate.HasValue)
                {
                    query = query.Where(t => t.Date >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(t => t.Date <= request.ToDate.Value);
                }

                if (!string.IsNullOrEmpty(request.Currency))
                {
                    query = query.Where(t => t.Currency == request.Currency);
                }

                if (request.FromAccountId.HasValue)
                {
                    query = query.Where(t => t.FromBankAccountId == request.FromAccountId.Value);
                }

                if (request.ToAccountId.HasValue)
                {
                    query = query.Where(t => t.ToBankAccountId == request.ToAccountId.Value);
                }

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(t => 
                        t.Description.Contains(request.SearchTerm) ||
                        t.DocumentNumber.Contains(request.SearchTerm) ||
                        t.Reference.Contains(request.SearchTerm) ||
                        t.Notes.Contains(request.SearchTerm));
                }

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply pagination and ordering
                var transfers = await query
                    .OrderByDescending(t => t.Date)
                    .ThenByDescending(t => t.Id)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                // Map to DTOs
                var transferDtos = transfers.Select(transfer => new TransferDto
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
                }).ToList();

                var result = new PagedResult<TransferDto>(transferDtos, totalCount, request.Page, request.PageSize);
                return Result.Success(result);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<PagedResult<TransferDto>>($"Error retrieving transfers: {ex.Message}");
            }
        }
    }
}