using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class DeleteTransferCommandHandler : ICommandHandler<DeleteTransferCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public DeleteTransferCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transfer = await _context.Transfers
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.Company == request.Company, cancellationToken);

                if (transfer == null)
                {
                    return Result<bool>.Failure("Transfer not found");
                }

                // Check if transfer can be deleted (only pending transfers can be deleted)
                if (transfer.Status != TransferStatus.Pending)
                {
                    return Result<bool>.Failure("Only pending transfers can be deleted");
                }

                // Remove related ledger entries first
                var ledgerEntries = await _context.LedgerEntries
                    .Where(le => le.DocumentNumber == transfer.DocumentNumber && le.Company == request.Company)
                    .ToListAsync(cancellationToken);

                _context.LedgerEntries.RemoveRange(ledgerEntries);

                // Remove the transfer
                _context.Transfers.Remove(transfer);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (System.Exception ex)
            {
                return Result<bool>.Failure($"Error deleting transfer: {ex.Message}");
            }
        }
    }
}