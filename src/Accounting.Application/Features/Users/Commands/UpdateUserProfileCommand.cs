using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Users.Commands
{
    public class UpdateUserProfileCommand : ICommand<Result<UserProfileDto>>
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
    {
        private readonly IAccountingDbContext _context;

        public UpdateUserProfileCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FindAsync(command.UserId);
                
                if (user == null)
                    return Result.Failure<UserProfileDto>("User not found");

                // Check if email is already taken by another user
                if (!string.IsNullOrEmpty(command.Email) && command.Email != user.Email)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == command.Email && u.Id != command.UserId, cancellationToken);
                    
                    if (existingUser != null)
                        return Result.Failure<UserProfileDto>("Email is already taken");
                }

                // Update user properties
                if (!string.IsNullOrEmpty(command.Email))
                    user.Email = command.Email;
                
                if (!string.IsNullOrEmpty(command.FirstName))
                    user.FirstName = command.FirstName;
                
                if (!string.IsNullOrEmpty(command.LastName))
                    user.LastName = command.LastName;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicture = user.ProfilePicture
                });
            }
            catch (System.Exception ex)
            {
                return Result.Failure<UserProfileDto>($"Error updating profile: {ex.Message}");
            }
        }
    }
}