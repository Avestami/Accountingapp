using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Authorization;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Accounts.Commands;
using Accounting.Application.Features.Accounts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ICommandHandler<CreateAccountCommand, Result<int>> _createAccountHandler;
        private readonly ICommandHandler<UpdateAccountCommand, Result<bool>> _updateAccountHandler;
        private readonly ICommandHandler<DeleteAccountCommand, Result<bool>> _deleteAccountHandler;
        private readonly IQueryHandler<GetAccountsQuery, Result<List<AccountDto>>> _getAccountsHandler;
        private readonly IQueryHandler<GetAccountByIdQuery, Result<AccountDto>> _getAccountByIdHandler;

        public AccountsController(
            ICommandHandler<CreateAccountCommand, Result<int>> createAccountHandler,
            ICommandHandler<UpdateAccountCommand, Result<bool>> updateAccountHandler,
            ICommandHandler<DeleteAccountCommand, Result<bool>> deleteAccountHandler,
            IQueryHandler<GetAccountsQuery, Result<List<AccountDto>>> getAccountsHandler,
            IQueryHandler<GetAccountByIdQuery, Result<AccountDto>> getAccountByIdHandler)
        {
            _createAccountHandler = createAccountHandler;
            _updateAccountHandler = updateAccountHandler;
            _deleteAccountHandler = deleteAccountHandler;
            _getAccountsHandler = getAccountsHandler;
            _getAccountByIdHandler = getAccountByIdHandler;
        }

        [HttpGet]
        [Permission(Permissions.AccountsView)]
        public async Task<ActionResult<List<AccountDto>>> GetAccounts(
            [FromQuery] bool? isActive = null,
            [FromQuery] int? parentAccountId = null,
            [FromQuery] string accountType = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAccountsQuery
            {
                IsActive = isActive,
                ParentAccountId = parentAccountId,
                AccountType = accountType
            };

            var result = await _getAccountsHandler.Handle(query, cancellationToken);
            
            if (result.IsSuccess)
                return Ok(result.Value);
            
            return BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        [Permission(Permissions.AccountsView)]
        public async Task<ActionResult<AccountDto>> GetAccount(int id, CancellationToken cancellationToken = default)
        {
            var query = new GetAccountByIdQuery { Id = id };
            var result = await _getAccountByIdHandler.Handle(query, cancellationToken);
            
            if (result.IsSuccess)
                return Ok(result.Value);
            
            return NotFound(result.Error);
        }

        [HttpPost]
        [Permission(Permissions.AccountsCreate)]
        public async Task<ActionResult<int>> CreateAccount([FromBody] CreateAccountCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _createAccountHandler.Handle(command, cancellationToken);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetAccount), new { id = result.Value }, result.Value);
            
            return BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        [Permission(Permissions.AccountsEdit)]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] UpdateAccountCommand command, CancellationToken cancellationToken = default)
        {
            command.Id = id;
            var result = await _updateAccountHandler.Handle(command, cancellationToken);
            
            if (result.IsSuccess)
                return NoContent();
            
            return BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        [Permission(Permissions.AccountsDelete)]
        public async Task<ActionResult> DeleteAccount(int id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteAccountCommand { Id = id };
            var result = await _deleteAccountHandler.Handle(command, cancellationToken);
            
            if (result.IsSuccess)
                return NoContent();
            
            return BadRequest(result.Error);
        }
    }
}