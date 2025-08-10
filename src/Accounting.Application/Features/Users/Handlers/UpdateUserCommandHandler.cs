using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Users.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Users.Handlers
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Id == request.Id);

                if (user == null)
                {
                    return Result.Failure<UserDto>("کاربر یافت نشد");
                }

                // Check if username is taken by another user
                var existingUser = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.Id != request.Id);

                if (existingUser != null)
                {
                    return Result.Failure<UserDto>("نام کاربری قبلاً استفاده شده است");
                }

                // Check if email is taken by another user
                var existingEmail = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != request.Id);

                if (existingEmail != null)
                {
                    return Result.Failure<UserDto>("ایمیل قبلاً استفاده شده است");
                }

                // Update user properties
                user.Username = request.Username;
                user.Email = request.Email;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Role = request.Role;
                user.IsActive = request.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

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
                return Result.Failure<UserDto>($"خطا در به‌روزرسانی کاربر: {ex.Message}");
            }
        }
    }
}