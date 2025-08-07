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
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(AccountingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Accounts
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<Account> GetByCodeAsync(string code)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountCode == code);
        }

        public async Task<IEnumerable<Account>> GetByTypeAsync(AccountType type)
        {
            return await _context.Accounts
                .Where(a => a.Type == type)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Accounts.Where(a => a.AccountCode == code);
            
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.AccountCode)
                .ToListAsync();
        }
    }
}