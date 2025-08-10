using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using MediatR;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetSalesReportQuery : IRequest<Result<SalesReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
    }
}