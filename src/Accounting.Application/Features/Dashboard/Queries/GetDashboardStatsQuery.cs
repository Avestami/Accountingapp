using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsQuery : IQuery<Result<DashboardStatsDto>>
    {
        /// <summary>
        /// Start date for the dashboard statistics period
        /// </summary>
        public DateTime? FromDate { get; set; }
        
        /// <summary>
        /// End date for the dashboard statistics period
        /// </summary>
        public DateTime? ToDate { get; set; }
        
        /// <summary>
        /// Number of days to look back if FromDate is not specified
        /// </summary>
        [Range(1, 365, ErrorMessage = "Period must be between 1 and 365 days")]
        public int Period { get; set; } = 30; // Default to last 30 days
        
        /// <summary>
        /// Currency filter for revenue calculations (optional)
        /// </summary>
        public string? Currency { get; set; }
        
        /// <summary>
        /// Include detailed chart data in the response
        /// </summary>
        public bool IncludeChartData { get; set; } = true;
    }
}