using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class DeleteCostCommandHandler : ICommandHandler<DeleteCostCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public DeleteCostCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteCostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cost = await _context.Costs
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.Company == request.Company, cancellationToken);

                if (cost == null)
                {
                    return Result.Failure<bool>("Cost not found");
                }

                // Check if cost is referenced in any vouchers or other entities
                // This is a business rule to prevent deletion of referenced costs
                var hasReferences = await _context.VoucherEntries
                    .AnyAsync(ve => ve.Description.Contains($"Cost ID: {cost.Id}"), cancellationToken);

                if (hasReferences)
                {
                    return Result.Failure<bool>("Cannot delete cost that is referenced in vouchers");
                }

                _context.Costs.Remove(cost);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<bool>($"Error deleting cost: {ex.Message}");
            }
        }
    }
}