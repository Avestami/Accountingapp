using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Users.Queries;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Users.Handlers
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Id == request.Id);

                if (user == null)
                {
                    return Result.Failure<UserDto>("کاربر یافت نشد");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
                };

                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<UserDto>($"خطا در دریافت کاربر: {ex.Message}");
            }
        }
    }
}