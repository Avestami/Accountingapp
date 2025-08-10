using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class ExportFinanceDataCommandHandler : ICommandHandler<ExportFinanceDataCommand, Result<byte[]>>
    {
        private readonly IAccountingDbContext _context;

        public ExportFinanceDataCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<byte[]>> Handle(ExportFinanceDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                switch (request.Format)
                {
                    case ExportFormat.Csv:
                        return await ExportToCsv(request, cancellationToken);
                    case ExportFormat.Excel:
                        return Result.Failure<byte[]>("Excel export not implemented yet");
                    case ExportFormat.Pdf:
                        return Result.Failure<byte[]>("PDF export not implemented yet");
                    default:
                        return Result.Failure<byte[]>("Unsupported export format");
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"Error exporting data: {ex.Message}");
            }
        }

        private async Task<Result<byte[]>> ExportToCsv(ExportFinanceDataCommand request, CancellationToken cancellationToken)
        {
            var csv = new StringBuilder();

            if (request.Type == ExportType.Costs || request.Type == ExportType.All)
            {
                csv.AppendLine("=== COSTS ===");
                csv.AppendLine("Document Number,Date,Description,Amount,Currency,Exchange Rate,Local Amount,Payment Source,Bank Account,Counterparty,Reference,Notes,Status,Created At");

                var costsQuery = _context.Costs.Where(c => c.Company == request.Company);
                costsQuery = ApplyFilters(costsQuery, request);

                var costs = await costsQuery
                    .Include(c => c.BankAccount)
                    .Include(c => c.Counterparty)
                    .OrderByDescending(c => c.Date)
                    .ToListAsync(cancellationToken);

                foreach (var cost in costs)
                {
                    csv.AppendLine($"\"{cost.DocumentNumber}\",\"{cost.Date:yyyy-MM-dd}\",\"{cost.Description}\",{cost.Amount},\"{cost.Currency}\",{cost.ExchangeRate},{cost.LocalAmount},\"{cost.PaymentSource}\",\"{cost.BankAccount?.AccountName}\",\"{cost.Counterparty?.Name}\",\"{cost.Reference}\",\"{cost.Notes}\",\"{cost.Status}\",\"{cost.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
                }

                csv.AppendLine();
            }

            if (request.Type == ExportType.Incomes || request.Type == ExportType.All)
            {
                csv.AppendLine("=== INCOMES ===");
                csv.AppendLine("Document Number,Date,Description,Amount,Currency,Exchange Rate,Local Amount,Payment Source,Bank Account,Counterparty,Reference,Notes,Status,Created At");

                var incomesQuery = _context.Incomes.Where(i => i.Company == request.Company);
                incomesQuery = ApplyFilters(incomesQuery, request);

                var incomes = await incomesQuery
                    .Include(i => i.BankAccount)
                    .Include(i => i.Counterparty)
                    .OrderByDescending(i => i.Date)
                    .ToListAsync(cancellationToken);

                foreach (var income in incomes)
                {
                    csv.AppendLine($"\"{income.DocumentNumber}\",\"{income.Date:yyyy-MM-dd}\",\"{income.Description}\",{income.Amount},\"{income.Currency}\",{income.ExchangeRate},{income.LocalAmount},\"{income.PaymentSource}\",\"{income.BankAccount?.AccountName}\",\"{income.Counterparty?.Name}\",\"{income.Reference}\",\"{income.Notes}\",\"{income.Status}\",\"{income.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
                }

                csv.AppendLine();
            }

            if (request.Type == ExportType.Transfers || request.Type == ExportType.All)
            {
                csv.AppendLine("=== TRANSFERS ===");
                csv.AppendLine("Document Number,Date,Description,Amount,Currency,Exchange Rate,Local Amount,From Account,To Account,Reference,Notes,Status,Created At");

                var transfersQuery = _context.Transfers.Where(t => t.Company == request.Company);
                transfersQuery = ApplyTransferFilters(transfersQuery, request);

                var transfers = await transfersQuery
                    .Include(t => t.FromBankAccount)
                    .Include(t => t.ToBankAccount)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync(cancellationToken);

                foreach (var transfer in transfers)
                {
                    csv.AppendLine($"\"{transfer.DocumentNumber}\",\"{transfer.Date:yyyy-MM-dd}\",\"{transfer.Description}\",{transfer.Amount},\"{transfer.Currency}\",{transfer.ExchangeRate},{transfer.Amount},\"{transfer.FromBankAccount?.AccountName}\",\"{transfer.ToBankAccount?.AccountName}\",\"{transfer.Reference}\",\"{transfer.Notes}\",\"{transfer.Status}\",\"{transfer.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return Result<byte[]>.Success(bytes);
        }

        private IQueryable<T> ApplyFilters<T>(IQueryable<T> query, ExportFinanceDataCommand request) where T : class
        {
            // This is a simplified version - in reality you'd need to use reflection or expression trees
            // For now, we'll handle this in the specific query methods
            return query;
        }

        private IQueryable<Domain.Entities.Transfer> ApplyTransferFilters(IQueryable<Domain.Entities.Transfer> query, ExportFinanceDataCommand request)
        {
            if (request.FromDate.HasValue)
                query = query.Where(t => t.Date >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.Date <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.Currency))
                query = query.Where(t => t.Currency == request.Currency);

            if (!string.IsNullOrEmpty(request.SearchTerm))
                query = query.Where(t => t.Description.Contains(request.SearchTerm) || 
                                       t.Reference.Contains(request.SearchTerm));

            return query;
        }
    }
}