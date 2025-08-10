using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Accounts.Commands;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Accounts.Handlers
{
    public class DeleteAccountCommandHandler : ICommandHandler<DeleteAccountCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.Id);
            if (account == null)
            {
                return Result.Failure<bool>("Account not found");
            }

            // Check if account has child accounts
            var childAccounts = await _unitOfWork.Accounts.FindAsync(a => a.ParentAccountId == request.Id);
            if (childAccounts.Any())
            {
                return Result.Failure<bool>("Cannot delete account with child accounts");
            }

            // Check if account has transactions
            var hasTransactions = await _unitOfWork.VoucherEntries.FirstOrDefaultAsync(ve => ve.AccountId == request.Id);
            if (hasTransactions != null)
            {
                return Result.Failure<bool>("Cannot delete account with existing transactions");
            }

            await _unitOfWork.Accounts.DeleteAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}