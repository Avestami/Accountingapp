using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Users.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Users.Handlers
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if username already exists
                var existingUser = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.Company == request.Company);

                if (existingUser != null)
                {
                    return Result.Failure<UserDto>("نام کاربری قبلاً استفاده شده است");
                }

                // Check if email already exists
                var existingEmail = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Company == request.Company);

                if (existingEmail != null)
                {
                    return Result.Failure<UserDto>("ایمیل قبلاً استفاده شده است");
                }

                // Parse role - already UserRole enum, no need to parse
                var userRole = request.Role;

                // Create new user
                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // Hash password
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = userRole,
                    Company = request.Company,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
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

                return Result.Success(userDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<UserDto>($"خطا در ایجاد کاربر: {ex.Message}");
            }
        }
    }
}