using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using MediatR;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetProfitLossReportQuery : IRequest<Result<ProfitLossReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}