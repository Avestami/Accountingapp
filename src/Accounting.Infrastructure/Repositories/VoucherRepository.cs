using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Infrastructure.Data;
using Accounting.Application.Interfaces;

namespace Accounting.Infrastructure.Repositories
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(AccountingDbContext context) : base(context)
        {
        }

        public async Task<Voucher> GetVoucherWithDetailsAsync(int id)
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Include(v => v.Ticket)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<(IEnumerable<Voucher> Items, int TotalCount)> GetVouchersAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            VoucherType? type = null,
            VoucherStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string currency = null,
            int? ticketId = null,
            int? createdByUserId = null,
            string sortBy = null,
            bool sortDescending = false)
        {
            var query = _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Include(v => v.Ticket)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(v => v.VoucherNumber.Contains(searchTerm) ||
                                        v.Description.Contains(searchTerm) ||
                                        v.Reference.Contains(searchTerm));
            }

            if (type.HasValue)
            {
                query = query.Where(v => v.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(v => v.VoucherDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(v => v.VoucherDate <= toDate.Value);
            }

            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(v => v.Currency == currency);
            }

            if (ticketId.HasValue)
            {
                query = query.Where(v => v.TicketId == ticketId.Value);
            }

            if (createdByUserId.HasValue)
            {
                query = query.Where(v => v.CreatedByUserId == createdByUserId.Value);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "vouchernumber":
                        query = sortDescending ? query.OrderByDescending(v => v.VoucherNumber) : query.OrderBy(v => v.VoucherNumber);
                        break;
                    case "type":
                        query = sortDescending ? query.OrderByDescending(v => v.Type) : query.OrderBy(v => v.Type);
                        break;
                    case "status":
                        query = sortDescending ? query.OrderByDescending(v => v.Status) : query.OrderBy(v => v.Status);
                        break;
                    case "voucherdate":
                        query = sortDescending ? query.OrderByDescending(v => v.VoucherDate) : query.OrderBy(v => v.VoucherDate);
                        break;
                    case "amount":
                        query = sortDescending ? query.OrderByDescending(v => v.Amount) : query.OrderBy(v => v.Amount);
                        break;
                    case "currency":
                        query = sortDescending ? query.OrderByDescending(v => v.Currency) : query.OrderBy(v => v.Currency);
                        break;
                    case "createdat":
                        query = sortDescending ? query.OrderByDescending(v => v.CreatedAt) : query.OrderBy(v => v.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(v => v.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(v => v.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<string> GetNextVoucherNumberAsync(VoucherType type)
        {
            var prefix = GetVoucherPrefix(type);
            var year = DateTime.Now.Year;
            var yearSuffix = year.ToString().Substring(2); // Get last 2 digits of year

            var lastVoucher = await _context.Vouchers
                .Where(v => v.Type == type && v.VoucherNumber.StartsWith(prefix))
                .OrderByDescending(v => v.VoucherNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastVoucher != null)
            {
                // Extract number from voucher number (e.g., "JV-23-0001" -> "0001")
                var parts = lastVoucher.VoucherNumber.Split('-');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{yearSuffix}-{nextNumber:D4}";
        }

        public async Task<bool> VoucherNumberExistsAsync(string voucherNumber)
        {
            return await _context.Vouchers
                .AnyAsync(v => v.VoucherNumber == voucherNumber);
        }

        public async Task<IEnumerable<Voucher>> GetVouchersByTicketIdAsync(int ticketId)
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Where(v => v.TicketId == ticketId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetVouchersByStatusAsync(VoucherStatus status)
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Where(v => v.Status == status)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetVouchersByUserAsync(int userId)
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Where(v => v.CreatedByUserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetPendingVouchersAsync()
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Where(v => v.Status == VoucherStatus.Pending)
                .OrderBy(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetPostedVouchersAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Vouchers
                .Include(v => v.Entries)
                    .ThenInclude(e => e.Account)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .Where(v => v.Status == VoucherStatus.Posted && 
                           v.PostedDate.HasValue &&
                           v.PostedDate.Value.Date >= fromDate.Date &&
                           v.PostedDate.Value.Date <= toDate.Date)
                .OrderBy(v => v.PostedDate)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private string GetVoucherPrefix(VoucherType type)
        {
            return type switch
            {
                VoucherType.Income => "IV",
                VoucherType.Expense => "EV",
                VoucherType.Transfer => "TV",
                _ => "GV" // General Voucher
            };
        }
    }
}