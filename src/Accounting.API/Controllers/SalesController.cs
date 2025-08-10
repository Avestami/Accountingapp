using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Threading.Tasks;
using System.Security.Claims;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Application.Features.Tickets.Queries;
using Accounting.Application.Common.Authorization;
using Accounting.Domain.Enums;
using System;
using Accounting.Application.DTOs;
using Accounting.Application.Common.Models;
using static Accounting.Application.Features.Tickets.Queries.GetSalesStatisticsQuery;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all sales documents (tickets) with filtering and pagination
        /// </summary>
        [HttpGet("documents")]
        public async Task<ActionResult<Result<PagedResult<TicketDto>>>> GetSalesDocuments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] TicketStatus? status = null,
            [FromQuery] string? serviceType = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortDirection = "desc")
        {
            var query = new GetTicketsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Status = status,
                Type = serviceType != null ? Enum.Parse<TicketType>(serviceType) : null,
                FromDate = dateFrom,
                ToDate = dateTo,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get a specific sales document by ID
        /// </summary>
        [HttpGet("documents/{id}")]
        public async Task<ActionResult<Result<TicketDto>>> GetSalesDocument(int id)
        {
            var query = new GetTicketByIdQuery(id);
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create a new sales document (ticket)
        /// </summary>
        [HttpPost("documents")]
        [RequirePermission(Permission.CreateTicket)]
        public async Task<ActionResult<Result<TicketDto>>> CreateSalesDocument([FromBody] CreateTicketCommand command)
        {
            command.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var result = await _mediator.Send(command);
            return result.IsSuccess ? CreatedAtAction(nameof(GetSalesDocument), new { id = result.Value.Id }, result) : BadRequest(result);
        }

        /// <summary>
        /// Update an existing sales document
        /// </summary>
        [HttpPut("documents/{id}")]
        [RequirePermission(Permission.UpdateTicket)]
        public async Task<ActionResult<Result<TicketDto>>> UpdateSalesDocument(int id, [FromBody] UpdateTicketCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            command.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete a sales document
        /// </summary>
        [HttpDelete("documents/{id}")]
        [RequirePermission(Permission.DeleteTicket)]
        public async Task<ActionResult<Result<bool>>> DeleteSalesDocument(int id)
        {
            var command = new DeleteTicketCommand(id)
            {
                DeletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Issue a ticket (change status to issued)
        /// </summary>
        [HttpPost("documents/{id}/issue")]
        [RequirePermission(Permission.IssueTicket)]
        public async Task<ActionResult<Result<TicketDto>>> IssueTicket(int id)
        {
            var command = new IssueTicketCommand(id)
            {
                IssuedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cancel a ticket
        /// </summary>
        [HttpPost("documents/{id}/cancel")]
        [RequirePermission(Permission.CancelTicket)]
        public async Task<ActionResult<Result<TicketDto>>> CancelTicket(int id, [FromBody] CancelTicketCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            command.CancelledBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get sales statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<Result<SalesStatisticsDto>>> GetSalesStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetSalesStatisticsQuery
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}