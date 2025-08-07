using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Data;
using Accounting.Infrastructure.Repositories;

namespace Accounting.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccountingDbContext _context;
        private IDbContextTransaction _transaction;
        
        // Repository properties
        public IRepository<User> Users { get; private set; }
        public IRepository<Ticket> Tickets { get; private set; }
        public IRepository<TicketItem> TicketItems { get; private set; }
        public IRepository<Voucher> Vouchers { get; private set; }
        public IRepository<VoucherEntry> VoucherEntries { get; private set; }
        public IRepository<Account> Accounts { get; private set; }
        public IRepository<FxTransaction> FxTransactions { get; private set; }
        public IRepository<FxConsumption> FxConsumptions { get; private set; }
        public IRepository<DocumentNumber> DocumentNumbers { get; private set; }
        public IRepository<AuditLog> AuditLogs { get; private set; }
        public IRepository<Report> Reports { get; private set; }

        public UnitOfWork(AccountingDbContext context)
        {
            _context = context;
            
            // Initialize repositories
            Users = new Repository<User>(_context);
            Tickets = new Repository<Ticket>(_context);
            TicketItems = new Repository<TicketItem>(_context);
            Vouchers = new Repository<Voucher>(_context);
            VoucherEntries = new Repository<VoucherEntry>(_context);
            Accounts = new Repository<Account>(_context);
            FxTransactions = new Repository<FxTransaction>(_context);
            FxConsumptions = new Repository<FxConsumption>(_context);
            DocumentNumbers = new Repository<DocumentNumber>(_context);
            AuditLogs = new Repository<AuditLog>(_context);
            Reports = new Repository<Report>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            await BeginTransactionAsync();
            try
            {
                var result = await operation();
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await BeginTransactionAsync();
            try
            {
                await operation();
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}