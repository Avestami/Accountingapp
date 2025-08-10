using System;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.AuditLogs.Queries
{
    public class GetAuditLogsQuery : IQuery<Result<PagedResult<AuditLogDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public AuditAction? Action { get; set; }
        public int? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchTerm { get; set; }
    }
}