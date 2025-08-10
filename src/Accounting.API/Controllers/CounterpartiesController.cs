using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Counterparties.Commands;
using Accounting.Application.Features.Counterparties.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CounterpartiesController : ControllerBase
    {
        private readonly ICommandHandler<CreateCounterpartyCommand, Result<CounterpartyDto>> _createHandler;
        private readonly ICommandHandler<UpdateCounterpartyCommand, Result<CounterpartyDto>> _updateHandler;
        private readonly ICommandHandler<DeleteCounterpartyCommand, Result<bool>> _deleteHandler;
        private readonly IQueryHandler<GetCounterpartyByIdQuery, Result<CounterpartyDto>> _getByIdHandler;
        private readonly IQueryHandler<GetCounterpartiesQuery, Result<Accounting.Application.Common.Models.PagedResult<CounterpartyDto>>> _getHandler;

        public CounterpartiesController(
            ICommandHandler<CreateCounterpartyCommand, Result<CounterpartyDto>> createHandler,
            ICommandHandler<UpdateCounterpartyCommand, Result<CounterpartyDto>> updateHandler,
            ICommandHandler<DeleteCounterpartyCommand, Result<bool>> deleteHandler,
            IQueryHandler<GetCounterpartyByIdQuery, Result<CounterpartyDto>> getByIdHandler,
            IQueryHandler<GetCounterpartiesQuery, Result<Accounting.Application.Common.Models.PagedResult<CounterpartyDto>>> getHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _getByIdHandler = getByIdHandler;
            _getHandler = getHandler;
        }

        [HttpPost]
        public async Task<ActionResult<CounterpartyDto>> Create([FromBody] CreateCounterpartyCommand command)
        {
            var result = await _createHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<Accounting.Application.Common.Models.PagedResult<CounterpartyDto>>> Get([FromQuery] GetCounterpartiesQuery query)
        {
            var result = await _getHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CounterpartyDto>> GetById(int id)
        {
            var query = new GetCounterpartyByIdQuery(id);
            var result = await _getByIdHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CounterpartyDto>> Update(int id, [FromBody] UpdateCounterpartyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _updateHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteCounterpartyCommand(id);
            var result = await _deleteHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}