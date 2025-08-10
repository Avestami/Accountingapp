using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Locations.Commands;
using Accounting.Application.Features.Locations.Queries;
using Accounting.Application.Features.Locations.Handlers;
using Accounting.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly CreateLocationCommandHandler _createLocationHandler;
        private readonly UpdateLocationCommandHandler _updateLocationHandler;
        private readonly DeleteLocationCommandHandler _deleteLocationHandler;
        private readonly GetLocationsQueryHandler _getLocationsHandler;
        private readonly GetLocationByIdQueryHandler _getLocationByIdHandler;
        private readonly GetCountriesQueryHandler _getCountriesHandler;
        private readonly GetCitiesByCountryQueryHandler _getCitiesByCountryHandler;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(
            CreateLocationCommandHandler createLocationHandler,
            UpdateLocationCommandHandler updateLocationHandler,
            DeleteLocationCommandHandler deleteLocationHandler,
            GetLocationsQueryHandler getLocationsHandler,
            GetLocationByIdQueryHandler getLocationByIdHandler,
            GetCountriesQueryHandler getCountriesHandler,
            GetCitiesByCountryQueryHandler getCitiesByCountryHandler,
            ILogger<LocationsController> logger)
        {
            _createLocationHandler = createLocationHandler;
            _updateLocationHandler = updateLocationHandler;
            _deleteLocationHandler = deleteLocationHandler;
            _getLocationsHandler = getLocationsHandler;
            _getLocationByIdHandler = getLocationByIdHandler;
            _getCountriesHandler = getCountriesHandler;
            _getCitiesByCountryHandler = getCitiesByCountryHandler;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationDto>>> GetLocations(
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? parentId = null)
        {
            var query = new GetLocationsQuery
            {
                Type = type,
                IsActive = isActive,
                SearchTerm = searchTerm,
                ParentId = parentId
            };

            var result = await _getLocationsHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            _logger.LogError("Error retrieving locations: {ErrorMessage}", result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            var query = new GetLocationByIdQuery(id);
            var result = await _getLocationByIdHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error == "Location not found")
            {
                return NotFound(new { message = result.Error });
            }

            _logger.LogError("Error retrieving location with ID {LocationId}: {ErrorMessage}", id, result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CreateLocationCommand
            {
                Name = request.Name,
                Type = request.Type,
                Code = request.Code,
                ParentId = request.ParentId,
                IsActive = request.IsActive
            };

            var result = await _createLocationHandler.Handle(command, CancellationToken.None);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetLocation), new { id = result.Value.Id }, result.Value);
            }

            if (result.Error.Contains("not found") || result.Error.Contains("Invalid"))
            {
                return BadRequest(new { message = result.Error });
            }

            _logger.LogError("Error creating location: {ErrorMessage}", result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new UpdateLocationCommand
            {
                Id = id,
                Name = request.Name,
                Type = request.Type,
                Code = request.Code,
                ParentId = request.ParentId,
                IsActive = request.IsActive
            };

            var result = await _updateLocationHandler.Handle(command, CancellationToken.None);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error == "Location not found")
            {
                return NotFound(new { message = result.Error });
            }

            if (result.Error.Contains("not found") || result.Error.Contains("cannot"))
            {
                return BadRequest(new { message = result.Error });
            }

            _logger.LogError("Error updating location with ID {LocationId}: {ErrorMessage}", id, result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            var command = new DeleteLocationCommand(id);
            var result = await _deleteLocationHandler.Handle(command, CancellationToken.None);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            if (result.Error == "Location not found")
            {
                return NotFound(new { message = result.Error });
            }

            if (result.Error.Contains("Cannot delete"))
            {
                return BadRequest(new { message = result.Error });
            }

            _logger.LogError("Error deleting location with ID {LocationId}: {ErrorMessage}", id, result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<LocationDto>>> GetCountries([FromQuery] bool? isActive = null)
        {
            var query = new GetCountriesQuery { IsActive = isActive };
            var result = await _getCountriesHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            _logger.LogError("Error retrieving countries: {ErrorMessage}", result.Error);
            return StatusCode(500, new { message = result.Error });
        }

        [HttpGet("cities/{countryId}")]
        public async Task<ActionResult<List<LocationDto>>> GetCitiesByCountry(int countryId, [FromQuery] bool? isActive = null)
        {
            var query = new GetCitiesByCountryQuery(countryId)
            { 
                IsActive = isActive
            };
            var result = await _getCitiesByCountryHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            _logger.LogError("Error retrieving cities for country {CountryId}: {ErrorMessage}", countryId, result.Error);
            return StatusCode(500, new { message = result.Error });
        }
    }
}

public class CreateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
}