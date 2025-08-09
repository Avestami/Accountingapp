using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using System;

namespace Accounting.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsQuery : IQuery<Result<DashboardStatsDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Period { get; set; } = 30; // Default to last 30 days
    }
}