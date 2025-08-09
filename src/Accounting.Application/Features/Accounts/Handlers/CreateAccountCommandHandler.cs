using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Accounts.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Accounts.Handlers
{
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Validate account code uniqueness
            var existingAccount = await _unitOfWork.Accounts.FirstOrDefaultAsync(a => a.AccountCode == request.AccountCode);
            if (existingAccount != null)
            {
                return Result.Failure<int>("Account code already exists");
            }

            // Validate parent account exists if specified
            if (request.ParentAccountId.HasValue)
            {
                var parentAccount = await _unitOfWork.Accounts.GetByIdAsync(request.ParentAccountId.Value);
                if (parentAccount == null)
                {
                    return Result.Failure<int>("Parent account not found");
                }
            }

            var account = new Account
            {
                AccountCode = request.AccountCode,
                AccountName = request.AccountName,
                Description = request.Description,
                Type = request.Type,
                ParentAccountId = request.ParentAccountId,
                Currency = request.Currency,
                IsActive = request.IsActive,
                Balance = 0
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(account.Id);
        }
    }
}