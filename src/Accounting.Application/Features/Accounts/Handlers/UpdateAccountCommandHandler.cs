using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Accounts.Commands;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Accounts.Handlers
{
    public class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.Id);
            if (account == null)
            {
                return Result.Failure<bool>("Account not found");
            }

            // Validate account code uniqueness (excluding current account)
            var existingAccount = await _unitOfWork.Accounts.FirstOrDefaultAsync(a => a.AccountCode == request.AccountCode && a.Id != request.Id);
            if (existingAccount != null)
            {
                return Result.Failure<bool>("Account code already exists");
            }

            // Validate parent account exists if specified
            if (request.ParentAccountId.HasValue)
            {
                var parentAccount = await _unitOfWork.Accounts.GetByIdAsync(request.ParentAccountId.Value);
                if (parentAccount == null)
                {
                    return Result.Failure<bool>("Parent account not found");
                }

                // Prevent circular reference
                if (request.ParentAccountId == request.Id)
                {
                    return Result.Failure<bool>("Account cannot be its own parent");
                }
            }

            account.AccountCode = request.AccountCode;
            account.AccountName = request.AccountName;
            account.Description = request.Description;
            account.Type = request.Type;
            account.ParentAccountId = request.ParentAccountId;
            account.Currency = request.Currency;
            account.IsActive = request.IsActive;

            await _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}