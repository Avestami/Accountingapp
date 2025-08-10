using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Domain.Entities;
using Accounting.Application.Interfaces;
using Accounting.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        private readonly AccountingDbContext _context;
    private readonly ILogger<BanksController> _logger;

    public BanksController(AccountingDbContext context, ILogger<BanksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Accounting.Application.Common.Models.PagedResult<Bank>>> GetBanks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string sortBy = "Name",
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                var query = _context.Banks.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(b => 
                        b.Name.Contains(searchTerm) ||
                        (b.SwiftCode != null && b.SwiftCode.Contains(searchTerm)) ||
                        (b.Address != null && b.Address.Contains(searchTerm)));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(b => b.IsActive == isActive.Value);
                }

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "name" => sortDirection.ToLower() == "desc" 
                        ? query.OrderByDescending(b => b.Name)
                        : query.OrderBy(b => b.Name),
                    "swiftcode" => sortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.SwiftCode)
                        : query.OrderBy(b => b.SwiftCode),
                    "createdat" => sortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.CreatedAt)
                        : query.OrderBy(b => b.CreatedAt),
                    _ => query.OrderBy(b => b.Name)
                };

                var totalCount = await query.CountAsync();
                var banks = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new Accounting.Application.Common.Models.PagedResult<Bank>(
                    banks,
                    totalCount,
                    page,
                    pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving banks");
                return StatusCode(500, new { message = "An error occurred while retrieving banks" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bank>> GetBank(int id)
        {
            try
            {
                var bank = await _context.Banks
                    .Include(b => b.BankAccounts)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (bank == null)
                {
                    return NotFound(new { message = "Bank not found" });
                }

                return Ok(bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bank with ID {BankId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the bank" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Bank>> CreateBank([FromBody] CreateBankRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bank = new Bank
                {
                    Name = request.Name,
                    SwiftCode = request.SwiftCode,
                    Address = request.Address,
                    Phone = request.Phone,
                    Website = request.Website,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name
                };

                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBank), new { id = bank.Id }, bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bank");
                return StatusCode(500, new { message = "An error occurred while creating the bank" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Bank>> UpdateBank(int id, [FromBody] UpdateBankRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bank = await _context.Banks.FindAsync(id);
                if (bank == null)
                {
                    return NotFound(new { message = "Bank not found" });
                }

                bank.Name = request.Name;
                bank.SwiftCode = request.SwiftCode;
                bank.Address = request.Address;
                bank.Phone = request.Phone;
                bank.Website = request.Website;
                bank.IsActive = request.IsActive;
                bank.UpdatedAt = DateTime.UtcNow;
                bank.UpdatedBy = User.Identity?.Name;

                await _context.SaveChangesAsync();

                return Ok(bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bank with ID {BankId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the bank" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBank(int id)
        {
            try
            {
                var bank = await _context.Banks.FindAsync(id);
                if (bank == null)
                {
                    return NotFound(new { message = "Bank not found" });
                }

                // Check if bank has associated bank accounts
                var hasAccounts = await _context.BankAccounts.AnyAsync(ba => ba.BankId == id);
                if (hasAccounts)
                {
                    return BadRequest(new { message = "Cannot delete bank with associated bank accounts" });
                }

                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bank with ID {BankId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the bank" });
            }
        }
    }

    public class CreateBankRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? SwiftCode { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateBankRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? SwiftCode { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}