using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Users.Commands
{
    public class DeleteProfilePictureCommand : ICommand<Result<bool>>
    {
        public int UserId { get; set; }
    }

    public class DeleteProfilePictureCommandHandler : ICommandHandler<DeleteProfilePictureCommand, Result<bool>>
    {
        private readonly IAccountingDbContext _context;

        public DeleteProfilePictureCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteProfilePictureCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FindAsync(command.UserId);
                
                if (user == null)
                    return Result.Failure<bool>("User not found");

                if (string.IsNullOrEmpty(user.ProfilePicture))
                    return Result.Failure<bool>("No profile picture to delete");

                // Delete physical file
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pictures");
                var fileName = Path.GetFileName(user.ProfilePicture);
                var filePath = Path.Combine(uploadsPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Update database
                user.ProfilePicture = null;
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<bool>($"Error deleting profile picture: {ex.Message}");
            }
        }
    }
}