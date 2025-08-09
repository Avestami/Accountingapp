using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using BCrypt.Net;

namespace Accounting.Application.Features.Users.Commands
{
    public class ChangePasswordCommand : ICommand<Result<bool>>
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public ChangePasswordCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(command.CurrentPassword) || string.IsNullOrEmpty(command.NewPassword))
                    return Result.Failure<bool>("Current password and new password are required");

                if (command.NewPassword.Length < 6)
                    return Result.Failure<bool>("New password must be at least 6 characters long");

                var user = await _context.Users.FindAsync(command.UserId);
                
                if (user == null)
                    return Result.Failure<bool>("User not found");

                // Verify current password
                if (!VerifyPassword(command.CurrentPassword, user.PasswordHash))
                    return Result.Failure<bool>("Current password is incorrect");

                // Hash new password
                user.PasswordHash = HashPassword(command.NewPassword);
                
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(true);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<bool>($"Error changing password: {ex.Message}");
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}