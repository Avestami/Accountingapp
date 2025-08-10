#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.Features.Vouchers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Accounting.Application.Common.Authorization;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly ICommandHandler<CreateVoucherCommand, Result<VoucherDto>> _createVoucherHandler;
        private readonly ICommandHandler<UpdateVoucherCommand, Result<VoucherDto>> _updateVoucherHandler;
        private readonly ICommandHandler<DeleteVoucherCommand, Result<bool>> _deleteVoucherHandler;
        private readonly ICommandHandler<PostVoucherCommand, Result<VoucherDto>> _postVoucherHandler;
        private readonly ICommandHandler<ApproveVoucherCommand, Result<VoucherDto>> _approveVoucherHandler;
        private readonly ICommandHandler<RejectVoucherCommand, Result<VoucherDto>> _rejectVoucherHandler;
        private readonly ICommandHandler<SubmitVoucherCommand, Result<VoucherDto>> _submitVoucherHandler;
        private readonly ICommandHandler<CancelVoucherCommand, Result<VoucherDto>> _cancelVoucherHandler;
        private readonly IQueryHandler<GetVoucherByIdQuery, Result<VoucherDto>> _getVoucherByIdHandler;
        private readonly IQueryHandler<GetVouchersQuery, Result<Accounting.Application.Common.Models.PagedResult<VoucherDto>>> _getVouchersHandler;

        public VouchersController(
            ICommandHandler<CreateVoucherCommand, Result<VoucherDto>> createVoucherHandler,
            ICommandHandler<UpdateVoucherCommand, Result<VoucherDto>> updateVoucherHandler,
            ICommandHandler<DeleteVoucherCommand, Result<bool>> deleteVoucherHandler,
            ICommandHandler<PostVoucherCommand, Result<VoucherDto>> postVoucherHandler,
            ICommandHandler<ApproveVoucherCommand, Result<VoucherDto>> approveVoucherHandler,
            ICommandHandler<RejectVoucherCommand, Result<VoucherDto>> rejectVoucherHandler,
            ICommandHandler<SubmitVoucherCommand, Result<VoucherDto>> submitVoucherHandler,
            ICommandHandler<CancelVoucherCommand, Result<VoucherDto>> cancelVoucherHandler,
            IQueryHandler<GetVoucherByIdQuery, Result<VoucherDto>> getVoucherByIdHandler,
            IQueryHandler<GetVouchersQuery, Result<Accounting.Application.Common.Models.PagedResult<VoucherDto>>> getVouchersHandler)
        {
            _createVoucherHandler = createVoucherHandler;
            _updateVoucherHandler = updateVoucherHandler;
            _deleteVoucherHandler = deleteVoucherHandler;
            _postVoucherHandler = postVoucherHandler;
            _approveVoucherHandler = approveVoucherHandler;
            _rejectVoucherHandler = rejectVoucherHandler;
            _submitVoucherHandler = submitVoucherHandler;
            _cancelVoucherHandler = cancelVoucherHandler;
            _getVoucherByIdHandler = getVoucherByIdHandler;
            _getVouchersHandler = getVouchersHandler;
        }

        /// <summary>
        /// Get all vouchers with pagination and filtering
        /// </summary>
        [HttpGet]
        [Permission(Permissions.VouchersView)]
        public async Task<ActionResult<Accounting.Application.Common.Models.PagedResult<VoucherDto>>> GetVouchers([FromQuery] GetVouchersQuery query)
        {
            var result = await _getVouchersHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get a voucher by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VoucherDto>> GetVoucher(int id)
        {
            var query = new GetVoucherByIdQuery(id);
            var result = await _getVoucherByIdHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new voucher
        /// </summary>
        [HttpPost]
        [Permission(Permissions.VouchersCreate)]
        public async Task<ActionResult<VoucherDto>> CreateVoucher([FromBody] CreateVoucherCommand command)
        {
            var result = await _createVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetVoucher), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing voucher
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<VoucherDto>> UpdateVoucher(int id, [FromBody] UpdateVoucherCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _updateVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Delete a voucher
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVoucher(int id)
        {
            var command = new DeleteVoucherCommand { Id = id };
            var result = await _deleteVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Submit a voucher for approval
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<ActionResult<VoucherDto>> SubmitVoucher(int id, [FromBody] SubmitVoucherRequest request)
        {
            var command = new SubmitVoucherCommand 
            { 
                Id = id, 
                Notes = request?.Notes 
            };
            
            var result = await _submitVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Approve a voucher
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<VoucherDto>> ApproveVoucher(int id, [FromBody] ApproveVoucherRequest request)
        {
            var command = new ApproveVoucherCommand 
            { 
                Id = id, 
                Notes = request?.Notes 
            };
            
            var result = await _approveVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Reject a voucher
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<ActionResult<VoucherDto>> RejectVoucher(int id, [FromBody] RejectVoucherRequest request)
        {
            var command = new RejectVoucherCommand 
            { 
                Id = id, 
                Reason = request?.Reason ?? "No reason provided" 
            };
            
            var result = await _rejectVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Post a voucher to the general ledger
        /// </summary>
        [HttpPost("{id}/post")]
        public async Task<ActionResult<VoucherDto>> PostVoucher(int id, [FromBody] PostVoucherRequest request)
        {
            var command = new PostVoucherCommand 
            { 
                Id = id, 
                Notes = request?.Notes 
            };
            
            var result = await _postVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Cancel a voucher
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<VoucherDto>> CancelVoucher(int id, [FromBody] CancelVoucherRequest request)
        {
            var command = new CancelVoucherCommand 
            { 
                Id = id, 
                Reason = request?.Reason ?? "No reason provided" 
            };
            
            var result = await _cancelVoucherHandler.Handle(command, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }

    // Request models for action-specific endpoints
    public class SubmitVoucherRequest
    {
        public string? Notes { get; set; }
    }

    public class ApproveVoucherRequest
    {
        public string? Notes { get; set; }
    }

    public class RejectVoucherRequest
    {
        public string? Reason { get; set; }
    }

    public class PostVoucherRequest
    {
        public string? Notes { get; set; }
    }

    public class CancelVoucherRequest
    {
        public string? Reason { get; set; }
    }
}