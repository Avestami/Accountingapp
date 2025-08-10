using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using MediatR;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetBalanceSheetReportQuery : IRequest<Result<BalanceSheetReportDto>>
    {
        public DateTime AsOfDate { get; set; }
    }
}