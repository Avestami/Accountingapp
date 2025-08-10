using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using MediatR;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetFinancialReportQuery : IRequest<Result<FinancialReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Currency { get; set; } = "USD";
    }
}