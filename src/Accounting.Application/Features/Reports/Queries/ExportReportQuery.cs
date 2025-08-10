using Accounting.Application.Common.Models;
using MediatR;

namespace Accounting.Application.Features.Reports.Queries
{
    public class ExportReportQuery : IRequest<Result<byte[]>>
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Format { get; set; } = "excel";
    }
}