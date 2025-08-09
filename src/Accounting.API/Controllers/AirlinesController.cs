using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Airlines.Commands;
using Accounting.Application.Features.Airlines.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AirlinesController : ControllerBase
    {
        private readonly ICommandHandler<CreateAirlineCommand, Result<AirlineDto>> _createHandler;
        private readonly ICommandHandler<UpdateAirlineCommand, Result<AirlineDto>> _updateHandler;
        private readonly ICommandHandler<DeleteAirlineCommand, Result<bool>> _deleteHandler;
        private readonly IQueryHandler<GetAirlinesQuery, Result<PagedResult<AirlineDto>>> _getHandler;
        private readonly IQueryHandler<GetAirlineByIdQuery, Result<AirlineDto>> _getByIdHandler;

        public AirlinesController(
            ICommandHandler<CreateAirlineCommand, Result<AirlineDto>> createHandler,
            ICommandHandler<UpdateAirlineCommand, Result<AirlineDto>> updateHandler,
            ICommandHandler<DeleteAirlineCommand, Result<bool>> deleteHandler,
            IQueryHandler<GetAirlinesQuery, Result<PagedResult<AirlineDto>>> getHandler,
            IQueryHandler<GetAirlineByIdQuery, Result<AirlineDto>> getByIdHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _getHandler = getHandler;
            _getByIdHandler = getByIdHandler;
        }

        [HttpPost]
        public async Task<ActionResult<AirlineDto>> Create([FromBody] CreateAirlineCommand command)
        {
            // Set company from user claims
            command.Company = User.FindFirst("company")?.Value ?? "";
            
            var result = await _createHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AirlineDto>>> Get([FromQuery] GetAirlinesQuery query)
        {
            // Set company from user claims
            query.Company = User.FindFirst("company")?.Value ?? "";
            
            var result = await _getHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AirlineDto>> GetById(int id)
        {
            var query = new GetAirlineByIdQuery(id)
            {
                Company = User.FindFirst("company")?.Value ?? ""
            };
            
            var result = await _getByIdHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AirlineDto>> Update(int id, [FromBody] UpdateAirlineCommand command)
        {
            command.Id = id;
            command.Company = User.FindFirst("company")?.Value ?? "";
            
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
            var command = new DeleteAirlineCommand(id)
            {
                Company = User.FindFirst("company")?.Value ?? ""
            };
            
            var result = await _deleteHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}