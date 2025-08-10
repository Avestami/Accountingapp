using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Accounting.Application.Features.Users.Commands;
using Accounting.Application.Features.Users.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Common.Models;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all users with pagination and filtering
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<PagedResult<UserDto>>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetUsersQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Role = role,
                IsActive = isActive
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<UserDto>>> GetUser(int id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<UserDto>>> CreateUser([FromBody] CreateUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a user (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<bool>>> DeleteUser(int id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Activate/Deactivate a user
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<UserDto>>> ToggleUserStatus(int id, [FromBody] bool isActive)
        {
            // First get the user
            var getUserQuery = new GetUserByIdQuery { Id = id };
            var getUserResult = await _mediator.Send(getUserQuery);

            if (!getUserResult.IsSuccess)
            {
                return NotFound(getUserResult);
            }

            var user = getUserResult.Data;

            // Update only the status
            var updateCommand = new UpdateUserCommand
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Department = user.Department,
                Position = user.Position,
                IsActive = isActive
            };

            var result = await _mediator.Send(updateCommand);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}