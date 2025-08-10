using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Accounting.Application.Features.Reports.Queries;
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

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get financial report data
        /// </summary>
        [HttpGet("financial")]
        public async Task<ActionResult<Result<FinancialReportDto>>> GetFinancialReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? currency = null)
        {
            var query = new GetFinancialReportQuery
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get sales report data
        /// </summary>
        [HttpGet("sales")]
        public async Task<ActionResult<Result<SalesReportDto>>> GetSalesReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? reportType = "summary")
        {
            var query = new GetSalesReportQuery
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                ReportType = reportType
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get profit and loss report
        /// </summary>
        [HttpGet("profit-loss")]
        public async Task<ActionResult<Result<ProfitLossReportDto>>> GetProfitLossReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetProfitLossReportQuery
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get balance sheet report
        /// </summary>
        [HttpGet("balance-sheet")]
        public async Task<ActionResult<Result<BalanceSheetReportDto>>> GetBalanceSheet(
            [FromQuery] DateTime? asOfDate = null)
        {
            var query = new GetBalanceSheetReportQuery
            {
                AsOfDate = asOfDate ?? DateTime.Now
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Export report to Excel
        /// </summary>
        [HttpGet("export/{reportType}")]
        public async Task<ActionResult> ExportReport(
            string reportType,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string format = "excel")
        {
            var query = new ExportReportQuery
            {
                ReportType = reportType,
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                Format = format
            };

            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            var contentType = format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            var fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            
            return File(result.Data, contentType, fileName);
        }
    }
}