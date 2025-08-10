using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Domain.Entities;
using Accounting.Application.Interfaces;
using Accounting.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly AccountingDbContext _context;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(AccountingDbContext context, ILogger<LocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationDto>>> GetLocations(
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var query = _context.Locations
                    .Include(l => l.Parent)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(l => l.Type == type);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(l => l.IsActive == isActive.Value);
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(l => 
                        l.Name.Contains(searchTerm) ||
                        (l.Code != null && l.Code.Contains(searchTerm)));
                }

                var locations = await query
                    .OrderBy(l => l.Type)
                    .ThenBy(l => l.Name)
                    .ToListAsync();

                var locationDtos = locations.Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Type = l.Type,
                    Code = l.Code,
                    ParentId = l.ParentId,
                    ParentName = l.Parent?.Name,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                }).ToList();

                return Ok(locationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving locations");
                return StatusCode(500, new { message = "An error occurred while retrieving locations" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            try
            {
                var location = await _context.Locations
                    .Include(l => l.Parent)
                    .Include(l => l.Children)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (location == null)
                {
                    return NotFound(new { message = "Location not found" });
                }

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Type = location.Type,
                    Code = location.Code,
                    ParentId = location.ParentId,
                    ParentName = location.Parent?.Name,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt,
                    Children = location.Children.Select(c => new LocationDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.Type,
                        Code = c.Code,
                        IsActive = c.IsActive
                    }).ToList()
                };

                return Ok(locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location with ID {LocationId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the location" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate location type
                var validTypes = new[] { "country", "city", "airport" };
                if (!validTypes.Contains(request.Type.ToLower()))
                {
                    return BadRequest(new { message = "Invalid location type. Must be country, city, or airport." });
                }

                // Validate parent relationship
                if (request.ParentId.HasValue)
                {
                    var parent = await _context.Locations.FindAsync(request.ParentId.Value);
                    if (parent == null)
                    {
                        return BadRequest(new { message = "Parent location not found" });
                    }

                    // Business rule: countries cannot have parents
                    if (request.Type.ToLower() == "country")
                    {
                        return BadRequest(new { message = "Countries cannot have parent locations" });
                    }
                }

                var location = new Location
                {
                    Name = request.Name,
                    Type = request.Type.ToLower(),
                    Code = request.Code,
                    ParentId = request.ParentId,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name
                };

                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                // Reload with parent information
                await _context.Entry(location)
                    .Reference(l => l.Parent)
                    .LoadAsync();

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Type = location.Type,
                    Code = location.Code,
                    ParentId = location.ParentId,
                    ParentName = location.Parent?.Name,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };

                return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location");
                return StatusCode(500, new { message = "An error occurred while creating the location" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var location = await _context.Locations
                    .Include(l => l.Parent)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (location == null)
                {
                    return NotFound(new { message = "Location not found" });
                }

                // Validate parent relationship if changed
                if (request.ParentId.HasValue && request.ParentId != location.ParentId)
                {
                    var parent = await _context.Locations.FindAsync(request.ParentId.Value);
                    if (parent == null)
                    {
                        return BadRequest(new { message = "Parent location not found" });
                    }

                    // Prevent circular references
                    if (request.ParentId == id)
                    {
                        return BadRequest(new { message = "Location cannot be its own parent" });
                    }
                }

                location.Name = request.Name;
                location.Code = request.Code;
                location.ParentId = request.ParentId;
                location.IsActive = request.IsActive;
                location.UpdatedAt = DateTime.UtcNow;
                location.UpdatedBy = User.Identity?.Name;

                await _context.SaveChangesAsync();

                // Reload with parent information
                await _context.Entry(location)
                    .Reference(l => l.Parent)
                    .LoadAsync();

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Type = location.Type,
                    Code = location.Code,
                    ParentId = location.ParentId,
                    ParentName = location.Parent?.Name,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };

                return Ok(locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location with ID {LocationId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the location" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            try
            {
                var location = await _context.Locations
                    .Include(l => l.Children)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (location == null)
                {
                    return NotFound(new { message = "Location not found" });
                }

                // Check if location has children
                if (location.Children.Any())
                {
                    return BadRequest(new { message = "Cannot delete location with child locations" });
                }

                // Check if location is referenced by other entities (tickets, etc.)
                // Add additional checks here as needed based on your business logic

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location with ID {LocationId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the location" });
            }
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<LocationDto>>> GetCountries()
        {
            try
            {
                var countries = await _context.Locations
                    .Where(l => l.Type == "country" && l.IsActive)
                    .OrderBy(l => l.Name)
                    .ToListAsync();

                var countryDtos = countries.Select(c => new LocationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Code = c.Code,
                    IsActive = c.IsActive
                }).ToList();

                return Ok(countryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries");
                return StatusCode(500, new { message = "An error occurred while retrieving countries" });
            }
        }

        [HttpGet("cities/{countryId}")]
        public async Task<ActionResult<List<LocationDto>>> GetCitiesByCountry(int countryId)
        {
            try
            {
                var cities = await _context.Locations
                    .Where(l => l.Type == "city" && l.ParentId == countryId && l.IsActive)
                    .OrderBy(l => l.Name)
                    .ToListAsync();

                var cityDtos = cities.Select(c => new LocationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Code = c.Code,
                    ParentId = c.ParentId,
                    IsActive = c.IsActive
                }).ToList();

                return Ok(cityDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cities for country {CountryId}", countryId);
                return StatusCode(500, new { message = "An error occurred while retrieving cities" });
            }
        }
    }

    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<LocationDto> Children { get; set; } = new List<LocationDto>();
    }

    public class CreateLocationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateLocationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}