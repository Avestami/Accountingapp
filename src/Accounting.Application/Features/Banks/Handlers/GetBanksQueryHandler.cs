using System;
using System.Collections.Generic;
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
    public class GetBanksQueryHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetBanksQueryHandler> _logger;

        public GetBanksQueryHandler(IAccountingDbContext context, ILogger<GetBanksQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<PagedResult<BankDto>>> Handle(GetBanksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Banks.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(b => b.Name.Contains(request.SearchTerm) || 
                                           b.SwiftCode.Contains(request.SearchTerm));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(b => b.IsActive == request.IsActive.Value);
                }

                // Apply sorting
                query = request.SortBy.ToLower() switch
                {
                    "name" => request.SortDirection.ToLower() == "desc" 
                        ? query.OrderByDescending(b => b.Name)
                        : query.OrderBy(b => b.Name),
                    "swiftcode" => request.SortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.SwiftCode)
                        : query.OrderBy(b => b.SwiftCode),
                    "createdat" => request.SortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.CreatedAt)
                        : query.OrderBy(b => b.CreatedAt),
                    _ => query.OrderBy(b => b.Name)
                };

                var totalCount = await query.CountAsync(cancellationToken);
                var banks = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
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
                    .ToListAsync(cancellationToken);

                var result = new PagedResult<BankDto>(banks, totalCount, request.Page, request.PageSize);

                return Result<PagedResult<BankDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving banks");
                return Result.Failure<PagedResult<BankDto>>("An error occurred while retrieving banks");
            }
        }
    }
}