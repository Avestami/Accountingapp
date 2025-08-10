using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Domain.Entities;
using Accounting.Application.Interfaces;
using Accounting.Infrastructure.Data;
using Accounting.Application.DTOs;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Banks.Commands;
using Accounting.Application.Features.Banks.Queries;
using Accounting.Application.Features.Banks.Handlers;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BanksController : ControllerBase
    {
        private readonly CreateBankCommandHandler _createBankHandler;
        private readonly UpdateBankCommandHandler _updateBankHandler;
        private readonly DeleteBankCommandHandler _deleteBankHandler;
        private readonly GetBanksQueryHandler _getBanksHandler;
        private readonly GetBankByIdQueryHandler _getBankByIdHandler;
        private readonly ILogger<BanksController> _logger;

        public BanksController(
            CreateBankCommandHandler createBankHandler,
            UpdateBankCommandHandler updateBankHandler,
            DeleteBankCommandHandler deleteBankHandler,
            GetBanksQueryHandler getBanksHandler,
            GetBankByIdQueryHandler getBankByIdHandler,
            ILogger<BanksController> logger)
        {
            _createBankHandler = createBankHandler;
            _updateBankHandler = updateBankHandler;
            _deleteBankHandler = deleteBankHandler;
            _getBanksHandler = getBanksHandler;
            _getBankByIdHandler = getBankByIdHandler;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<BankDto>>> GetBanks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string sortBy = "Name",
            [FromQuery] string sortDirection = "asc")
        {
            var query = new GetBanksQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _getBanksHandler.Handle(query, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankDto>> GetBank(int id)
        {
            var query = new GetBankByIdQuery(id);
            var result = await _getBankByIdHandler.Handle(query, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }

        [HttpPost]
        public async Task<ActionResult<BankDto>> CreateBank([FromBody] CreateBankCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _createBankHandler.Handle(command, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBank), new { id = result.Value.Id }, result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BankDto>> UpdateBank(int id, [FromBody] UpdateBankCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _updateBankHandler.Handle(command, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBank(int id)
        {
            var command = new DeleteBankCommand(id);
            var result = await _deleteBankHandler.Handle(command, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }
    }
}