using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Accounts.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Accounts.Handlers
{
    public class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, Result<AccountDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AccountDto>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.Id);
            if (account == null)
            {
                return Result.Failure<AccountDto>("Account not found");
            }

            var accountDto = new AccountDto
            {
                Id = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.AccountName,
                Description = account.Description,
                Type = account.Type.ToString(),
                IsActive = account.IsActive,
                Balance = account.Balance,
                Currency = account.Currency,
                ParentAccountId = account.ParentAccountId,
                ParentAccountName = account.ParentAccount?.AccountName,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt ?? DateTime.MinValue
            };

            return Result.Success(accountDto);
        }
    }
}