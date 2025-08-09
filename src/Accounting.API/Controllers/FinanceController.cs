using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Threading.Tasks;
using System.Security.Claims;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Features.Finance.Queries;
using Accounting.Application.Common.Authorization;
using System;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new cost entry
        /// </summary>
        [HttpPost("costs")]
        [Permission(Permissions.FinanceCreate)]
        public async Task<IActionResult> CreateCost([FromBody] CreateCostCommand command)
        {
            // Set company from JWT token
            command.Company = User.FindFirst("company")?.Value ?? "demo";
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetCost), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Create a new income entry
        /// </summary>
        [HttpPost("incomes")]
        [Permission(Permissions.FinanceCreate)]
        public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeCommand command)
        {
            // Set company from JWT token
            command.Company = User.FindFirst("company")?.Value ?? "demo";
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetIncome), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Create a new transfer
        /// </summary>
        [HttpPost("transfers")]
        [Permission(Permissions.FinanceCreate)]
        public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferCommand command)
        {
            // Set company from JWT token
            command.Company = User.FindFirst("company")?.Value ?? "demo";
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetTransfer), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Get cost by ID
        /// </summary>
        [HttpGet("costs/{id}")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetCost(int id)
        {
            var query = new GetCostByIdQuery 
            { 
                Id = id, 
                Company = User.FindFirst("company")?.Value ?? "demo" 
            };
            
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get income by ID
        /// </summary>
        [HttpGet("incomes/{id}")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetIncome(int id)
        {
            var query = new GetIncomeByIdQuery 
            { 
                Id = id, 
                Company = User.FindFirst("company")?.Value ?? "demo" 
            };
            
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get transfer by ID
        /// </summary>
        [HttpGet("transfers/{id}")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetTransfer(int id)
        {
            var query = new GetTransferByIdQuery 
            { 
                Id = id, 
                Company = User.FindFirst("company")?.Value ?? "demo" 
            };
            
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }
            
            return Ok(result.Value);
        }

        /// <summary>
        /// Get all costs with pagination and filtering
        /// </summary>
        [HttpGet("costs")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetCosts(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] int? counterpartyId = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetCostsQuery
            {
                Page = page,
                PageSize = pageSize,
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency,
                CounterpartyId = counterpartyId,
                SearchTerm = searchTerm,
                Company = User.FindFirst("company")?.Value ?? "demo"
            };

            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all incomes with pagination and filtering
        /// </summary>
        [HttpGet("incomes")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetIncomes(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] int? counterpartyId = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetIncomesQuery
            {
                Page = page,
                PageSize = pageSize,
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency,
                CounterpartyId = counterpartyId,
                SearchTerm = searchTerm,
                Company = User.FindFirst("company")?.Value ?? "demo"
            };

            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all transfers
        /// </summary>
        [HttpGet("transfers")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> GetTransfers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] int? fromAccountId = null,
            [FromQuery] int? toAccountId = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetTransfersQuery
            {
                Page = page,
                PageSize = pageSize,
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                SearchTerm = searchTerm,
                Company = User.FindFirst("company")?.Value ?? "demo"
            };
            
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            
            return Ok(result.Value);
        }

        /// <summary>
        /// Export finance data in various formats
        /// </summary>
        [HttpPost("export")]
        [Permission(Permissions.FinanceView)]
        public async Task<IActionResult> ExportFinanceData([FromBody] ExportFinanceDataCommand command)
        {
            // Set company from JWT token
            command.Company = User.FindFirst("company")?.Value ?? "demo";
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            var fileName = $"finance_export_{DateTime.Now:yyyyMMdd_HHmmss}";
            var contentType = command.Format switch
            {
                ExportFormat.Csv => "text/csv",
                ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ExportFormat.Pdf => "application/pdf",
                _ => "application/octet-stream"
            };

            var fileExtension = command.Format switch
            {
                ExportFormat.Csv => ".csv",
                ExportFormat.Excel => ".xlsx",
                ExportFormat.Pdf => ".pdf",
                _ => ".bin"
            };

            return File(result.Value, contentType, $"{fileName}{fileExtension}");
        }
        /// <summary>
        /// Delete a cost
        /// </summary>
        [HttpDelete("costs/{id}")]
        [Permission(Permissions.FinanceEdit)]
        public async Task<IActionResult> DeleteCost(int id)
        {
            var command = new DeleteCostCommand 
            { 
                Id = id, 
                Company = User.FindFirst("company")?.Value ?? "demo" 
            };
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            
            return Ok(new { message = "Cost deleted successfully" });
        }

        /// <summary>
        /// Delete an income
        /// </summary>
        [HttpDelete("incomes/{id}")]
        [Permission(Permissions.FinanceEdit)]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var command = new DeleteIncomeCommand 
            { 
                Id = id, 
                Company = User.FindFirst("company")?.Value ?? "demo" 
            };
            
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            
            return Ok(new { message = "Income deleted successfully" });
        }
    }
}