using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.AuditLogs.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.AuditLogs.Handlers
{
    public class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<GetAuditLogsQueryHandler> _logger;

        public GetAuditLogsQueryHandler(IAccountingDbContext context, ILogger<GetAuditLogsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var auditLogsQuery = _context.AuditLogs
                    .Include(a => a.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(query.EntityName))
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.EntityName.Contains(query.EntityName));
                }

                if (!string.IsNullOrEmpty(query.EntityId))
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.EntityId == query.EntityId);
                }

                if (query.Action.HasValue)
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.Action == query.Action.Value);
                }

                if (query.UserId.HasValue)
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.UserId == query.UserId.Value);
                }

                if (query.StartDate.HasValue)
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.Timestamp >= query.StartDate.Value);
                }

                if (query.EndDate.HasValue)
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.Timestamp <= query.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(query.SearchTerm))
                {
                    auditLogsQuery = auditLogsQuery.Where(a => 
                        a.EntityName.Contains(query.SearchTerm) ||
                        a.Changes.Contains(query.SearchTerm) ||
                        a.AdditionalInfo.Contains(query.SearchTerm));
                }

                // Get total count
                var totalCount = await auditLogsQuery.CountAsync(cancellationToken);

                // Apply pagination and ordering
                var auditLogs = await auditLogsQuery
                    .OrderByDescending(a => a.Timestamp)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(a => new AuditLogDto
                    {
                        Id = a.Id,
                        EntityName = a.EntityName,
                        EntityId = a.EntityId,
                        Action = a.Action,
                        OldValues = a.OldValues,
                        NewValues = a.NewValues,
                        Changes = a.Changes,
                        Timestamp = a.Timestamp,
                        IpAddress = a.IpAddress,
                        UserAgent = a.UserAgent,
                        AdditionalInfo = a.AdditionalInfo,
                        UserId = a.UserId,
                        UserName = a.User != null ? a.User.Username : null,
                        TicketId = a.TicketId,
                        VoucherId = a.VoucherId
                    })
                    .ToListAsync(cancellationToken);

                var pagedResult = new PagedResult<AuditLogDto>(
                    auditLogs,
                    totalCount,
                    query.Page,
                    query.PageSize
                );

                return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return Result.Failure<PagedResult<AuditLogDto>>($"Error retrieving audit logs: {ex.Message}");
            }
        }
    }
}