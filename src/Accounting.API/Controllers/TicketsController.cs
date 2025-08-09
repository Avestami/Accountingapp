using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Tickets.Commands;
using Accounting.Application.Features.Tickets.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ICommandHandler<CreateTicketCommand, Result<TicketDto>> _createTicketHandler;
        private readonly ICommandHandler<UpdateTicketCommand, Result<TicketDto>> _updateTicketHandler;
        private readonly ICommandHandler<DeleteTicketCommand, Result<bool>> _deleteTicketHandler;
        private readonly ICommandHandler<IssueTicketCommand, Result<TicketDto>> _issueTicketHandler;
        private readonly ICommandHandler<CancelTicketCommand, Result<TicketDto>> _cancelTicketHandler;
        private readonly IQueryHandler<GetTicketByIdQuery, Result<TicketDto>> _getTicketByIdHandler;
        private readonly IQueryHandler<GetTicketsQuery, Result<PagedResult<TicketDto>>> _getTicketsHandler;

        public TicketsController(
            ICommandHandler<CreateTicketCommand, Result<TicketDto>> createTicketHandler,
            ICommandHandler<UpdateTicketCommand, Result<TicketDto>> updateTicketHandler,
            ICommandHandler<DeleteTicketCommand, Result<bool>> deleteTicketHandler,
            ICommandHandler<IssueTicketCommand, Result<TicketDto>> issueTicketHandler,
            ICommandHandler<CancelTicketCommand, Result<TicketDto>> cancelTicketHandler,
            IQueryHandler<GetTicketByIdQuery, Result<TicketDto>> getTicketByIdHandler,
            IQueryHandler<GetTicketsQuery, Result<PagedResult<TicketDto>>> getTicketsHandler)
        {
            _createTicketHandler = createTicketHandler;
            _updateTicketHandler = updateTicketHandler;
            _deleteTicketHandler = deleteTicketHandler;
            _issueTicketHandler = issueTicketHandler;
            _cancelTicketHandler = cancelTicketHandler;
            _getTicketByIdHandler = getTicketByIdHandler;
            _getTicketsHandler = getTicketsHandler;
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketCommand command)
        {
            var result = await _createTicketHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetTicket), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Get all tickets with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<TicketDto>>> GetTickets([FromQuery] GetTicketsQuery query)
        {
            var result = await _getTicketsHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get a ticket by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            var query = new GetTicketByIdQuery(id);
            var result = await _getTicketByIdHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Update an existing ticket
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TicketDto>> UpdateTicket(int id, [FromBody] UpdateTicketCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _updateTicketHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Delete a ticket
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            var command = new DeleteTicketCommand(id);
            var result = await _deleteTicketHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Issue a ticket (change status from Unissued to Issued)
        /// </summary>
        [HttpPost("{id}/issue")]
        public async Task<ActionResult<TicketDto>> IssueTicket(int id, [FromBody] IssueTicketRequest request)
        {
            var command = new IssueTicketCommand(id, request.Notes);
            var result = await _issueTicketHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Cancel a ticket
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<TicketDto>> CancelTicket(int id, [FromBody] CancelTicketRequest request)
        {
            var command = new CancelTicketCommand(id, request.Reason, request.Notes);
            var result = await _cancelTicketHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }

    // Request models for action-specific endpoints
    public class IssueTicketRequest
    {
        public string? Notes { get; set; }
    }

    public class CancelTicketRequest
    {
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}