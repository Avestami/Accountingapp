using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Common.Queries;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetSalesReportQuery : IQuery<Result<SalesReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
    }
}