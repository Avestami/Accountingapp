using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using System;

namespace Accounting.Application.Features.Reports.Queries
{
    public class ExportReportQuery : IQuery<Result<byte[]>>
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Format { get; set; } = "excel";
    }
}