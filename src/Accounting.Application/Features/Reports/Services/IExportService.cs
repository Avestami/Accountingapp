using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Reports.Services
{
    public interface IExportService
    {
        Task<Result<byte[]>> ExportToPdfAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class;
        Task<Result<byte[]>> ExportToExcelAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class;
        Task<Result<byte[]>> ExportToCsvAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class;
        Task<Result<byte[]>> ExportToJsonAsync<T>(T data, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class;
        
        // Specialized report exports
        Task<Result<byte[]>> ExportSalesReportAsync(SalesReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default);
        Task<Result<byte[]>> ExportFinancialReportAsync(FinancialReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default);
        Task<Result<byte[]>> ExportProfitLossReportAsync(ProfitLossReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default);
        Task<Result<byte[]>> ExportBalanceSheetReportAsync(BalanceSheetReportDto report, ExportOptionsDto options, CancellationToken cancellationToken = default);
        
        // Bulk export
        Task<Result<byte[]>> ExportMultipleReportsAsync(List<object> reports, ExportOptionsDto options, CancellationToken cancellationToken = default);
        
        // Template-based exports
        Task<Result<byte[]>> ExportWithTemplateAsync<T>(T data, string templatePath, ExportOptionsDto options, CancellationToken cancellationToken = default) where T : class;
    }
}