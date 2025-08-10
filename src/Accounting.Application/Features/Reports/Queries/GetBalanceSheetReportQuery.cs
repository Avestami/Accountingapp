using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Common.Queries;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetBalanceSheetReportQuery : IQuery<Result<BalanceSheetReportDto>>
    {
        public DateTime AsOfDate { get; set; }
    }
}