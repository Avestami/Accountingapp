using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IWebHostEnvironment environment, ILogger<UserProfileController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var username = GetCurrentUsername();
                
                // Mock user profile data - in real app, query from database
                var userProfile = new
                {
                    id = userId,
                    username = username,
                    firstName = username == "admin" ? "مدیر" : "کاربر",
                    lastName = username == "admin" ? "سیستم" : "عادی",
                    fullName = username == "admin" ? "مدیر سیستم" : "کاربر عادی",
                    email = $"{username}@travel-accounting.com",
                    role = username == "admin" ? "admin" : "user",
                    company = "demo",
                    profilePicture = GetUserProfilePicture(userId),
                    isActive = true,
                    createdAt = DateTime.UtcNow.AddDays(-30),
                    lastLoginAt = DateTime.UtcNow
                };

                return Ok(new { success = true, data = userProfile });
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

                // In real app, update database
                var updatedProfile = new
                {
                    id = userId,
                    username = GetCurrentUsername(),
                    firstName = request.FirstName,
                    lastName = request.LastName,
                    fullName = $"{request.FirstName} {request.LastName}",
                    email = request.Email,
                    role = GetCurrentUserRole(),
                    company = "demo",
                    profilePicture = GetUserProfilePicture(userId),
                    isActive = true,
                    createdAt = DateTime.UtcNow.AddDays(-30),
                    lastLoginAt = DateTime.UtcNow
                };

                return Ok(new { success = true, data = updatedProfile });
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

                // In real app, validate current password and update in database
                // For now, just return success
                return Ok(new { success = true, message = "رمز عبور با موفقیت تغییر یافت" });
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
                var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "profile-pictures");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Delete old profile picture if exists
                var oldPicture = GetUserProfilePicture(userId);
                if (!string.IsNullOrEmpty(oldPicture))
                {
                    var oldFileName = Path.GetFileName(oldPicture);
                    var oldFilePath = Path.Combine(uploadsPath, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var profilePictureUrl = $"/uploads/profile-pictures/{fileName}";
                
                // In real app, save to database
                SaveUserProfilePicture(userId, profilePictureUrl);

                return Ok(new { 
                    success = true, 
                    data = new { profilePictureUrl },
                    message = "تصویر پروفایل با موفقیت آپلود شد"
                });
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
                var profilePicture = GetUserProfilePicture(userId);
                
                if (string.IsNullOrEmpty(profilePicture))
                {
                    return BadRequest(new { success = false, error = "تصویر پروفایل وجود ندارد" });
                }

                var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "profile-pictures");
                var fileName = Path.GetFileName(profilePicture);
                var filePath = Path.Combine(uploadsPath, fileName);

                // Delete file if exists
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // In real app, update database
                SaveUserProfilePicture(userId, null);

                return Ok(new { 
                    success = true, 
                    message = "تصویر پروفایل حذف شد"
                });
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

        // Mock storage for profile pictures - in real app, use database
        private static readonly Dictionary<int, string> _userProfilePictures = new();

        private string GetUserProfilePicture(int userId)
        {
            _userProfilePictures.TryGetValue(userId, out var profilePicture);
            return profilePicture;
        }

        private void SaveUserProfilePicture(int userId, string profilePictureUrl)
        {
            if (string.IsNullOrEmpty(profilePictureUrl))
            {
                _userProfilePictures.Remove(userId);
            }
            else
            {
                _userProfilePictures[userId] = profilePictureUrl;
            }
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