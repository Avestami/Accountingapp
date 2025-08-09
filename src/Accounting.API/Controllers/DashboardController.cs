using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IQueryHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>> _getDashboardStatsHandler;

        public DashboardController(
            IQueryHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>> getDashboardStatsHandler)
        {
            _getDashboardStatsHandler = getDashboardStatsHandler;
        }

        /// <summary>
        /// Get dashboard statistics and KPIs
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats([FromQuery] GetDashboardStatsQuery query)
        {
            var result = await _getDashboardStatsHandler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}