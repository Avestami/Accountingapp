using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Accounts.Queries;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Accounts.Handlers
{
    public class GetAccountsQueryHandler : IQueryHandler<GetAccountsQuery, Result<List<AccountDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<AccountDto>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.Accounts.GetAllAsync();

            // Apply filters
            if (request.IsActive.HasValue)
            {
                accounts = accounts.Where(a => a.IsActive == request.IsActive.Value);
            }

            if (request.ParentAccountId.HasValue)
            {
                accounts = accounts.Where(a => a.ParentAccountId == request.ParentAccountId.Value);
            }

            if (!string.IsNullOrEmpty(request.AccountType))
            {
                if (System.Enum.TryParse<AccountType>(request.AccountType, out var accountType))
                {
                    accounts = accounts.Where(a => a.Type == accountType);
                }
            }

            var accountDtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                Description = a.Description,
                Type = a.Type.ToString(),
                IsActive = a.IsActive,
                Balance = a.Balance,
                Currency = a.Currency,
                ParentAccountId = a.ParentAccountId,
                ParentAccountName = a.ParentAccount?.AccountName,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt ?? DateTime.MinValue
            }).OrderBy(a => a.AccountCode).ToList();

            return Result.Success(accountDtos);
        }
    }
}