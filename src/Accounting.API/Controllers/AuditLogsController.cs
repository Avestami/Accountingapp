using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Accounting.Application.Common.Queries;
using Accounting.Application.Features.AuditLogs.Queries;
using Accounting.Application.Features.AuditLogs.Handlers;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly GetAuditLogsQueryHandler _getAuditLogsHandler;

        public AuditLogsController(GetAuditLogsQueryHandler getAuditLogsHandler)
        {
            _getAuditLogsHandler = getAuditLogsHandler;
        }

        /// <summary>
        /// Get audit logs with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
        {
            var result = await _getAuditLogsHandler.Handle(query, CancellationToken.None);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
    }
}