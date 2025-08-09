using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class DeleteIncomeCommandHandler : ICommandHandler<DeleteIncomeCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public DeleteIncomeCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _context.Incomes
                    .FirstOrDefaultAsync(i => i.Id == request.Id && i.Company == request.Company, cancellationToken);

                if (income == null)
                {
                    return Result.Failure<bool>("Income not found");
                }

                // Check if income is referenced in any vouchers or other entities
                // This is a business rule to prevent deletion of referenced incomes
                var hasReferences = await _context.VoucherEntries
                    .AnyAsync(ve => ve.Description.Contains($"Income ID: {income.Id}"), cancellationToken);

                if (hasReferences)
                {
                    return Result.Failure<bool>("Cannot delete income that is referenced in vouchers");
                }

                _context.Incomes.Remove(income);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(true);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<bool>($"Error deleting income: {ex.Message}");
            }
        }
    }
}