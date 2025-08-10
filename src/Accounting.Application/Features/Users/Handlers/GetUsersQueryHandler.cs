using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Users.Queries;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Users.Handlers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                var query = allUsers.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(u => 
                        u.Username.Contains(request.SearchTerm) ||
                        u.FirstName.Contains(request.SearchTerm) ||
                        u.LastName.Contains(request.SearchTerm) ||
                        u.Email.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrEmpty(request.Role))
                {
                    if (Enum.TryParse<UserRole>(request.Role, true, out var roleEnum))
                    {
                        query = query.Where(u => u.Role == roleEnum);
                    }
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == request.IsActive.Value);
                }

                var totalCount = query.Count();

                var users = query
                    .OrderBy(u => u.Username)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt ?? DateTime.UtcNow
                    })
                    .ToList();

                var pagedResult = new PagedResult<UserDto>(
                    users,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                return Result.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result.Failure<PagedResult<UserDto>>($"خطا در دریافت کاربران: {ex.Message}");
            }
        }
    }
}