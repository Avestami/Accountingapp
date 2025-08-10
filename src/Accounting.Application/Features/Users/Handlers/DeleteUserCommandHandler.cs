using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Users.Commands;
using Accounting.Application.Interfaces;

namespace Accounting.Application.Features.Users.Handlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Id == request.Id);

                if (user == null)
                {
                    return Result.Failure<bool>("کاربر یافت نشد");
                }

                // Soft delete - just mark as inactive
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"خطا در حذف کاربر: {ex.Message}");
            }
        }
    }
}