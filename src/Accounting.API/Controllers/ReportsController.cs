using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Accounting.Application.Features.Reports.Queries;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Features.Reports.Services;
using Accounting.Application.Common.Models;
using MediatR;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IReportGenerationService _reportGenerationService;
        private readonly IExportService _exportService;

        public ReportsController(
            IMediator mediator,
            IReportGenerationService reportGenerationService,
            IExportService exportService)
        {
            _mediator = mediator;
            _reportGenerationService = reportGenerationService;
            _exportService = exportService;
        }

        /// <summary>
        /// Get financial report data with advanced filtering
        /// </summary>
        [HttpGet("financial")]
        public async Task<ActionResult<Result<FinancialReportDto>>> GetFinancialReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] List<string>? categories = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = startDate ?? DateTime.Now.AddMonths(-1),
                DateTo = endDate ?? DateTime.Now,
                Currency = currency,
                SearchTerm = searchTerm,
                Categories = categories ?? new List<string>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _reportGenerationService.GenerateFinancialReportAsync(filter);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get sales report data with advanced filtering
        /// </summary>
        [HttpGet("sales")]
        public async Task<ActionResult<Result<SalesReportDto>>> GetSalesReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] List<string>? airlines = null,
            [FromQuery] List<string>? destinations = null,
            [FromQuery] List<string>? statusFilters = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = startDate ?? DateTime.Now.AddMonths(-1),
                DateTo = endDate ?? DateTime.Now,
                SearchTerm = searchTerm,
                Airlines = airlines ?? new List<string>(),
                Destinations = destinations ?? new List<string>(),
                StatusFilters = statusFilters ?? new List<string>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _reportGenerationService.GenerateSalesReportAsync(filter);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get profit and loss report with advanced filtering
        /// </summary>
        [HttpGet("profit-loss")]
        public async Task<ActionResult<Result<ProfitLossReportDto>>> GetProfitLossReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] List<string>? categories = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = startDate ?? DateTime.Now.AddMonths(-1),
                DateTo = endDate ?? DateTime.Now,
                Categories = categories ?? new List<string>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _reportGenerationService.GenerateProfitLossReportAsync(filter);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get balance sheet report
        /// </summary>
        [HttpGet("balance-sheet")]
        public async Task<ActionResult<Result<BalanceSheetReportDto>>> GetBalanceSheet(
            [FromQuery] DateTime? asOfDate = null)
        {
            var filter = new ReportFilterDto
            {
                DateTo = asOfDate ?? DateTime.Now
            };

            var result = await _reportGenerationService.GenerateBalanceSheetReportAsync(filter);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get cash flow report
        /// </summary>
        [HttpGet("cash-flow")]
        public async Task<ActionResult<Result<CashFlowReportDto>>> GetCashFlowReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = startDate ?? DateTime.Now.AddMonths(-1),
                DateTo = endDate ?? DateTime.Now
            };

            var result = await _reportGenerationService.GenerateCashFlowReportAsync(filter);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Export sales report in various formats
        /// </summary>
        [HttpPost("export/sales")]
        public async Task<ActionResult> ExportSalesReport([FromBody] ExportRequestDto request)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = request.StartDate,
                DateTo = request.EndDate,
                SearchTerm = request.SearchTerm,
                Airlines = request.Airlines ?? new List<string>(),
                Destinations = request.Destinations ?? new List<string>(),
                StatusFilters = request.StatusFilters ?? new List<string>()
            };

            var reportResult = await _reportGenerationService.GenerateSalesReportAsync(filter);
            if (!reportResult.IsSuccess)
                return BadRequest(reportResult);

            var exportOptions = new ExportOptionsDto
            {
                Format = request.Format,
                Title = "Sales Report",
                Subtitle = $"Period: {filter.DateFrom:yyyy-MM-dd} to {filter.DateTo:yyyy-MM-dd}",
                IncludeHeader = true,
                IncludeFooter = true,
                AutoFitColumns = true,
                WorksheetName = "Sales Report"
            };

            var exportResult = await _exportService.ExportSalesReportAsync(reportResult.Data, exportOptions);
            if (!exportResult.IsSuccess)
                return BadRequest(exportResult);

            var contentType = request.Format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => "application/octet-stream"
            };

            var extension = request.Format.ToLower() switch
            {
                "pdf" => "pdf",
                "excel" => "xlsx",
                "csv" => "csv",
                "json" => "json",
                _ => "bin"
            };

            var fileName = $"sales_report_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
            return File(exportResult.Data, contentType, fileName);
        }

        /// <summary>
        /// Export financial report in various formats
        /// </summary>
        [HttpPost("export/financial")]
        public async Task<ActionResult> ExportFinancialReport([FromBody] ExportRequestDto request)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = request.StartDate,
                DateTo = request.EndDate,
                SearchTerm = request.SearchTerm,
                Categories = request.Categories ?? new List<string>()
            };

            var reportResult = await _reportGenerationService.GenerateFinancialReportAsync(filter);
            if (!reportResult.IsSuccess)
                return BadRequest(reportResult);

            var exportOptions = new ExportOptionsDto
            {
                Format = request.Format,
                Title = "Financial Report",
                Subtitle = $"Period: {filter.DateFrom:yyyy-MM-dd} to {filter.DateTo:yyyy-MM-dd}",
                IncludeHeader = true,
                IncludeFooter = true,
                AutoFitColumns = true,
                WorksheetName = "Financial Report"
            };

            var exportResult = await _exportService.ExportFinancialReportAsync(reportResult.Data, exportOptions);
            if (!exportResult.IsSuccess)
                return BadRequest(exportResult);

            var contentType = request.Format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => "application/octet-stream"
            };

            var extension = request.Format.ToLower() switch
            {
                "pdf" => "pdf",
                "excel" => "xlsx",
                "csv" => "csv",
                "json" => "json",
                _ => "bin"
            };

            var fileName = $"financial_report_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
            return File(exportResult.Data, contentType, fileName);
        }

        /// <summary>
        /// Export profit/loss report in various formats
        /// </summary>
        [HttpPost("export/profit-loss")]
        public async Task<ActionResult> ExportProfitLossReport([FromBody] ExportRequestDto request)
        {
            var filter = new ReportFilterDto
            {
                DateFrom = request.StartDate,
                DateTo = request.EndDate,
                Categories = request.Categories ?? new List<string>()
            };

            var reportResult = await _reportGenerationService.GenerateProfitLossReportAsync(filter);
            if (!reportResult.IsSuccess)
                return BadRequest(reportResult);

            var exportOptions = new ExportOptionsDto
            {
                Format = request.Format,
                Title = "Profit & Loss Report",
                Subtitle = $"Period: {filter.DateFrom:yyyy-MM-dd} to {filter.DateTo:yyyy-MM-dd}",
                IncludeHeader = true,
                IncludeFooter = true,
                AutoFitColumns = true,
                WorksheetName = "Profit Loss Report"
            };

            var exportResult = await _exportService.ExportProfitLossReportAsync(reportResult.Data, exportOptions);
            if (!exportResult.IsSuccess)
                return BadRequest(exportResult);

            var contentType = request.Format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => "application/octet-stream"
            };

            var extension = request.Format.ToLower() switch
            {
                "pdf" => "pdf",
                "excel" => "xlsx",
                "csv" => "csv",
                "json" => "json",
                _ => "bin"
            };

            var fileName = $"profit_loss_report_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
            return File(exportResult.Data, contentType, fileName);
        }

        /// <summary>
        /// Export balance sheet report in various formats
        /// </summary>
        [HttpPost("export/balance-sheet")]
        public async Task<ActionResult> ExportBalanceSheetReport([FromBody] ExportRequestDto request)
        {
            var filter = new ReportFilterDto
            {
                DateTo = request.EndDate ?? DateTime.Now
            };

            var reportResult = await _reportGenerationService.GenerateBalanceSheetReportAsync(filter);
            if (!reportResult.IsSuccess)
                return BadRequest(reportResult);

            var exportOptions = new ExportOptionsDto
            {
                Format = request.Format,
                Title = "Balance Sheet",
                Subtitle = $"As of: {filter.DateTo:yyyy-MM-dd}",
                IncludeHeader = true,
                IncludeFooter = true,
                AutoFitColumns = true,
                WorksheetName = "Balance Sheet"
            };

            var exportResult = await _exportService.ExportBalanceSheetReportAsync(reportResult.Data, exportOptions);
            if (!exportResult.IsSuccess)
                return BadRequest(exportResult);

            var contentType = request.Format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => "application/octet-stream"
            };

            var extension = request.Format.ToLower() switch
            {
                "pdf" => "pdf",
                "excel" => "xlsx",
                "csv" => "csv",
                "json" => "json",
                _ => "bin"
            };

            var fileName = $"balance_sheet_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
            return File(exportResult.Data, contentType, fileName);
        }

        /// <summary>
        /// Legacy export endpoint for backward compatibility
        /// </summary>
        [HttpGet("export/{reportType}")]
        public async Task<ActionResult> ExportReport(
            string reportType,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string format = "excel")
        {
            var request = new ExportRequestDto
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                Format = format
            };

            return reportType.ToLower() switch
            {
                "sales" => await ExportSalesReport(request),
                "financial" => await ExportFinancialReport(request),
                "profit-loss" => await ExportProfitLossReport(request),
                "balance-sheet" => await ExportBalanceSheetReport(request),
                _ => BadRequest($"Unsupported report type: {reportType}")
            };
        }
    }

    public class ExportRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Format { get; set; } = "excel";
        public string? SearchTerm { get; set; }
        public List<string>? Categories { get; set; }
        public List<string>? Airlines { get; set; }
        public List<string>? Destinations { get; set; }
        public List<string>? StatusFilters { get; set; }
    }
}