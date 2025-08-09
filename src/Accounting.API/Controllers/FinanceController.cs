using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Threading.Tasks;
using System.Security.Claims;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Common.Authorization;

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
        [RequirePermission(Permissions.FinanceCreate)]
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
        [RequirePermission(Permissions.FinanceCreate)]
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
        /// Get cost by ID (placeholder)
        /// </summary>
        [HttpGet("costs/{id}")]
        [RequirePermission(Permissions.FinanceView)]
        public async Task<IActionResult> GetCost(int id)
        {
            // TODO: Implement GetCostQuery
            return Ok(new { id, message = "Cost details will be implemented" });
        }

        /// <summary>
        /// Get income by ID (placeholder)
        /// </summary>
        [HttpGet("incomes/{id}")]
        [RequirePermission(Permissions.FinanceView)]
        public async Task<IActionResult> GetIncome(int id)
        {
            // TODO: Implement GetIncomeQuery
            return Ok(new { id, message = "Income details will be implemented" });
        }

        /// <summary>
        /// Get all costs with pagination (placeholder)
        /// </summary>
        [HttpGet("costs")]
        [RequirePermission(Permissions.FinanceView)]
        public async Task<IActionResult> GetCosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implement GetCostsQuery
            return Ok(new { page, pageSize, message = "Costs list will be implemented" });
        }

        /// <summary>
        /// Get all incomes with pagination (placeholder)
        /// </summary>
        [HttpGet("incomes")]
        [RequirePermission(Permissions.FinanceView)]
        public async Task<IActionResult> GetIncomes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implement GetIncomesQuery
            return Ok(new { page, pageSize, message = "Incomes list will be implemented" });
        }
    }
}