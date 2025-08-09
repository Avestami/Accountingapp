using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Users.Commands
{
    public class UploadProfilePictureCommand : ICommand<Result<ProfilePictureDto>>
    {
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }

    public class UploadProfilePictureCommandHandler : ICommandHandler<UploadProfilePictureCommand, Result<ProfilePictureDto>>
    {
        private readonly IAccountingDbContext _context;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        public UploadProfilePictureCommandHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProfilePictureDto>> Handle(UploadProfilePictureCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validate file size
                if (command.Data.Length > MaxFileSize)
                {
                    return Result.Failure<ProfilePictureDto>("File size exceeds 5MB limit");
                }

                // Validate file type
                if (!System.Array.Exists(AllowedContentTypes, ct => ct == command.ContentType))
                {
                    return Result.Failure<ProfilePictureDto>("Invalid file type. Only JPEG, PNG, and GIF are allowed");
                }

                var user = await _context.Users.FindAsync(command.UserId);
                
                if (user == null)
                    return Result.Failure<ProfilePictureDto>("User not found");

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pictures");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileExtension = Path.GetExtension(command.FileName);
                var uniqueFileName = $"{command.UserId}_{System.Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldFilePath = Path.Combine(uploadsPath, Path.GetFileName(user.ProfilePicture));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Save new file
                await File.WriteAllBytesAsync(filePath, command.Data, cancellationToken);

                // Update user profile picture path
                user.ProfilePicture = $"/uploads/profile-pictures/{uniqueFileName}";
                
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(new ProfilePictureDto
                {
                    FileName = uniqueFileName,
                    ContentType = command.ContentType,
                    Data = command.Data
                });
            }
            catch (System.Exception ex)
            {
                return Result.Failure<ProfilePictureDto>($"Error uploading profile picture: {ex.Message}");
            }
        }
    }
}