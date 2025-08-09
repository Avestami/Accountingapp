using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Application.Interfaces;
using BCrypt.Net;

namespace Accounting.Infrastructure.Data
{
    public class AccountingDbContext : DbContext, IAccountingDbContext
    {
        public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
        {
        }
        
        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketItem> TicketItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherEntry> VoucherEntries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<FxTransaction> FxTransactions { get; set; }
        public DbSet<FxConsumption> FxConsumptions { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Origin> Origins { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<DocumentNumber> DocumentNumbers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Report> Reports { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configurations
            ConfigureBaseEntity(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureTicket(modelBuilder);
            ConfigureTicketItem(modelBuilder);
            ConfigureVoucher(modelBuilder);
            ConfigureVoucherEntry(modelBuilder);
            ConfigureAccount(modelBuilder);
            ConfigureFxTransaction(modelBuilder);
            ConfigureFxConsumption(modelBuilder);
            ConfigureTransfer(modelBuilder);
            ConfigureDocumentNumber(modelBuilder);
            ConfigureAuditLog(modelBuilder);
            ConfigureReport(modelBuilder);
            
            // Seed data
            SeedData(modelBuilder);
        }
        
        private void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            // Configure common properties for all entities inheriting from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");
                    
                modelBuilder.Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");
                    
                modelBuilder.Entity(entityType.ClrType)
                    .Property("IsDeleted")
                    .HasDefaultValue(false);
            }
        }
        
        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).HasConversion<int>();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.LastLoginAt);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
                entity.Property(e => e.RefreshTokenExpiryTime);
                entity.Property(e => e.ProfilePicture).HasMaxLength(500);
                
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
        
        private void ConfigureTicket(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Type).HasConversion<int>();
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.AssignedToUser)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasIndex(e => e.TicketNumber).IsUnique();
            });
        }
        
        private void ConfigureTicketItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PassengerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PassengerAge).HasMaxLength(10);
                entity.Property(e => e.FlightNumber).HasMaxLength(20);
                entity.Property(e => e.SeatNumber).HasMaxLength(10);
                entity.Property(e => e.Class).HasMaxLength(20);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Itinerary).HasMaxLength(1000);
                
                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Items)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Airline)
                    .WithMany()
                    .HasForeignKey(e => e.AirlineId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Origin)
                    .WithMany()
                    .HasForeignKey(e => e.OriginId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Destination)
                    .WithMany()
                    .HasForeignKey(e => e.DestinationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        
        private void ConfigureVoucher(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VoucherNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Reference).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.CreatedVouchers)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.PostedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.PostedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Ticket)
                    .WithMany()
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                entity.HasIndex(e => e.VoucherNumber).IsUnique();
            });
        }
        
        private void ConfigureVoucherEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VoucherEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TransactionType).HasConversion<int>();
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                
                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.Entries)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Account)
                    .WithMany(a => a.VoucherEntries)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        
        private void ConfigureAccount(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AccountName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                
                entity.HasOne(e => e.ParentAccount)
                    .WithMany(a => a.ChildAccounts)
                    .HasForeignKey(e => e.ParentAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasIndex(e => e.AccountCode).IsUnique();
            });
        }
        
        private void ConfigureFxTransaction(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FxTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,4)");
                entity.Property(e => e.RemainingAmount).HasColumnType("decimal(18,4)");
                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18,6)");
                entity.Property(e => e.TransactionType).HasConversion<int>();
                entity.Property(e => e.Company).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Reference).HasMaxLength(100);
                
                entity.HasMany(e => e.Consumptions)
                    .WithOne(c => c.FxTransaction)
                    .HasForeignKey(c => c.FxTransactionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        
        private void ConfigureFxConsumption(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FxConsumption>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConsumedAmount).HasColumnType("decimal(18,4)");
                entity.Property(e => e.ConsumedRate).HasColumnType("decimal(18,6)");
                entity.Property(e => e.ConsumedCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Company).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Reference).HasMaxLength(100);
                
                entity.HasOne(e => e.FxTransaction)
                    .WithMany(f => f.Consumptions)
                    .HasForeignKey(e => e.FxTransactionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        
        private void ConfigureTransfer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,4)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18,4)");
                entity.Property(e => e.FeeAmount).HasColumnType("decimal(18,4)");
                entity.Property(e => e.Reference).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Company).IsRequired().HasMaxLength(50);
                
                entity.HasOne(e => e.FromBankAccount)
                    .WithMany()
                    .HasForeignKey(e => e.FromBankAccountId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.ToBankAccount)
                    .WithMany()
                    .HasForeignKey(e => e.ToBankAccountId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasIndex(e => e.DocumentNumber).IsUnique();
            });
        }
        
        private void ConfigureDocumentNumber(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentNumber>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Prefix).HasMaxLength(10);
                entity.Property(e => e.Suffix).HasMaxLength(10);
                entity.Property(e => e.ResetPeriod).HasConversion<int>();
                
                entity.HasIndex(e => e.DocumentType).IsUnique();
            });
        }
        
        private void ConfigureAuditLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).HasConversion<int>();
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Ticket)
                    .WithMany()
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.AuditLogs)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
        
        private void ConfigureReport(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReportName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.FileName).HasMaxLength(255);
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                
                entity.HasOne(e => e.GeneratedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.GeneratedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@accounting.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    Company = "demo",
                    PasswordHash = "$2a$11$8K1p/a0dURXAMcGe71sS1.E7dvFECOlHHQ4ZaBbQ4QjQvgs1XlqRG", // "admin123"
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            // Seed document number configurations
            modelBuilder.Entity<DocumentNumber>().HasData(
                new DocumentNumber { Id = 1, DocumentType = "TICKET", Prefix = "TKT", CurrentNumber = 1, PadLength = 6, ResetPeriod = ResetPeriod.Yearly, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new DocumentNumber { Id = 2, DocumentType = "VOUCHER", Prefix = "VCH", CurrentNumber = 1, PadLength = 6, ResetPeriod = ResetPeriod.Yearly, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new DocumentNumber { Id = 3, DocumentType = "FXTRANS", Prefix = "FX", CurrentNumber = 1, PadLength = 6, ResetPeriod = ResetPeriod.Yearly, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            // Seed default accounts
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, AccountCode = "1000", AccountName = "Cash", Type = AccountType.Asset, Currency = "USD", Balance = 0, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Account { Id = 2, AccountCode = "1100", AccountName = "Bank Account", Type = AccountType.Asset, Currency = "USD", Balance = 0, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Account { Id = 3, AccountCode = "2000", AccountName = "Accounts Payable", Type = AccountType.Liability, Currency = "USD", Balance = 0, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Account { Id = 4, AccountCode = "5000", AccountName = "Travel Expenses", Type = AccountType.Expense, Currency = "USD", Balance = 0, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Account { Id = 5, AccountCode = "6000", AccountName = "FX Gain/Loss", Type = AccountType.Revenue, Currency = "USD", Balance = 0, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }
        
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}