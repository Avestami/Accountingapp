using System;
using System.Threading.Tasks;
using Accounting.Domain.Entities;

namespace Accounting.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties
        IRepository<User> Users { get; }
        IRepository<Ticket> Tickets { get; }
        IRepository<TicketItem> TicketItems { get; }
        IRepository<Voucher> Vouchers { get; }
        IRepository<VoucherEntry> VoucherEntries { get; }
        IRepository<Account> Accounts { get; }
        IRepository<FxTransaction> FxTransactions { get; }
        IRepository<FxConsumption> FxConsumptions { get; }
        IRepository<DocumentNumber> DocumentNumbers { get; }
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<Report> Reports { get; }
        IRepository<Location> Locations { get; }

        // Transaction management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // Bulk operations
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}