using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Domain.Entities;

namespace Accounting.Application.Interfaces
{
    public interface IAccountingDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Ticket> Tickets { get; set; }
        DbSet<TicketItem> TicketItems { get; set; }
        DbSet<Voucher> Vouchers { get; set; }
        DbSet<VoucherEntry> VoucherEntries { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Counterparty> Counterparties { get; set; }
        DbSet<Bank> Banks { get; set; }
        DbSet<BankAccount> BankAccounts { get; set; }
        DbSet<FxTransaction> FxTransactions { get; set; }
        DbSet<FxConsumption> FxConsumptions { get; set; }
        DbSet<Cost> Costs { get; set; }
        DbSet<Income> Incomes { get; set; }
        DbSet<Transfer> Transfers { get; set; }
        DbSet<LedgerEntry> LedgerEntries { get; set; }
        DbSet<DocumentNumber> DocumentNumbers { get; set; }
        DbSet<Airline> Airlines { get; set; }
        DbSet<Origin> Origins { get; set; }
        DbSet<Destination> Destinations { get; set; }
        DbSet<AuditLog> AuditLogs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}