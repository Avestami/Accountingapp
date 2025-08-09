using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Accounting.Application.Features.Users.Commands;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMediator _mediator;

        public UserProfileController(IWebHostEnvironment environment, ILogger<UserProfileController> logger, IMediator mediator)
        {
            _environment = environment;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new Accounting.Application.Features.Users.Queries.GetUserProfileQuery
                {
                    UserId = userId
                };

                var userProfile = await _mediator.Send(query);
                
                if (userProfile == null)
                {
                    return NotFound(new { success = false, error = "کاربر یافت نشد" });
                }

                var response = new
                {
                    id = userProfile.Id,
                    username = GetCurrentUsername(),
                    firstName = userProfile.FirstName,
                    lastName = userProfile.LastName,
                    fullName = $"{userProfile.FirstName} {userProfile.LastName}".Trim(),
                    email = userProfile.Email,
                    role = GetCurrentUserRole(),
                    company = "demo",
                    profilePicture = userProfile.ProfilePicture,
                    isActive = true,
                    createdAt = DateTime.UtcNow.AddDays(-30),
                    lastLoginAt = DateTime.UtcNow
                };

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { success = false, error = "خطا در دریافت اطلاعات پروفایل" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Validate input
                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    return BadRequest(new { success = false, error = "نام الزامی است" });
                }
                
                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    return BadRequest(new { success = false, error = "نام خانوادگی الزامی است" });
                }
                
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { success = false, error = "ایمیل الزامی است" });
                }

                var command = new Accounting.Application.Features.Users.Commands.UpdateUserProfileCommand
                {
                    UserId = userId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };

                var result = await _mediator.Send(command);
                
                if (result.IsSuccess)
                {
                    var updatedProfile = new
                    {
                        id = result.Value.Id,
                        username = GetCurrentUsername(),
                        firstName = result.Value.FirstName,
                        lastName = result.Value.LastName,
                        fullName = $"{result.Value.FirstName} {result.Value.LastName}".Trim(),
                        email = result.Value.Email,
                        role = GetCurrentUserRole(),
                        company = "demo",
                        profilePicture = result.Value.ProfilePicture,
                        isActive = true,
                        createdAt = DateTime.UtcNow.AddDays(-30),
                        lastLoginAt = DateTime.UtcNow
                    };

                    return Ok(new { success = true, data = updatedProfile });
                }
                else
                {
                    return BadRequest(new { success = false, error = result.Error });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { success = false, error = "خطا در به‌روزرسانی پروفایل" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                {
                    return BadRequest(new { success = false, error = "رمز عبور فعلی الزامی است" });
                }
                
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest(new { success = false, error = "رمز عبور جدید الزامی است" });
                }
                
                if (request.NewPassword.Length < 6)
                {
                    return BadRequest(new { success = false, error = "رمز عبور باید حداقل ۶ کاراکتر باشد" });
                }
                
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest(new { success = false, error = "رمز عبور جدید و تأیید آن مطابقت ندارند" });
                }

                var userId = GetCurrentUserId();
                var command = new ChangePasswordCommand
                {
                    UserId = userId,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword
                };

                var result = await _mediator.Send(command);
                
                if (result.IsSuccess)
                {
                    return Ok(new { success = true, message = "رمز عبور با موفقیت تغییر یافت" });
                }
                else
                {
                    return BadRequest(new { success = false, error = result.Error });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { success = false, error = "خطا در تغییر رمز عبور" });
            }
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, error = "فایل انتخاب نشده است" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, error = "فرمت فایل مجاز نیست. فقط JPG، PNG و GIF پذیرفته می‌شود" });
                }

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, error = "حجم فایل نباید بیشتر از ۵ مگابایت باشد" });
                }

                var userId = GetCurrentUserId();
                
                // Read file data
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                var command = new Accounting.Application.Features.Users.Commands.UploadProfilePictureCommand
                {
                    UserId = userId,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = fileData
                };

                var result = await _mediator.Send(command);
                
                if (result.IsSuccess)
                {
                    return Ok(new { 
                        success = true, 
                        data = new { profilePictureUrl = $"/uploads/profile-pictures/{result.Value.FileName}" },
                        message = "تصویر پروفایل با موفقیت آپلود شد"
                    });
                }
                else
                {
                    return BadRequest(new { success = false, error = result.Error });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture");
                return StatusCode(500, new { success = false, error = "خطا در آپلود تصویر" });
            }
        }

        [HttpDelete("profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var command = new Accounting.Application.Features.Users.Commands.DeleteProfilePictureCommand
                {
                    UserId = userId
                };

                var result = await _mediator.Send(command);
                
                if (result.IsSuccess)
                {
                    return Ok(new { 
                        success = true, 
                        message = "تصویر پروفایل حذف شد"
                    });
                }
                else
                {
                    return BadRequest(new { success = false, error = result.Error });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture");
                return StatusCode(500, new { success = false, error = "خطا در حذف تصویر" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        private string GetCurrentUsername()
        {
            var usernameClaim = User.FindFirst(ClaimTypes.Name);
            return usernameClaim?.Value ?? "admin";
        }

        private string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? "admin";
        }
    }

    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}