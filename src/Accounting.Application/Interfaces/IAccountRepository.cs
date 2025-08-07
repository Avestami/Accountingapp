using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<int> ids);
        Task<Account> GetByCodeAsync(string code);
        Task<IEnumerable<Account>> GetByTypeAsync(AccountType type);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<IEnumerable<Account>> GetActiveAccountsAsync();
    }
}