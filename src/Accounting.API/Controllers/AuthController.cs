using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "نام کاربری و رمز عبور الزامی است" });
                }

                // Simple captcha validation (stub for now)
                if (!string.IsNullOrEmpty(request.CaptchaToken) && !ValidateCaptcha(request.CaptchaToken))
                {
                    return BadRequest(new { message = "کد امنیتی نامعتبر است" });
                }

                // Mock user validation - in real app, check against database
                var user = ValidateUser(request.Username, request.Password, request.Company);
                if (user == null)
                {
                    return Unauthorized(new { message = "نام کاربری یا رمز عبور اشتباه است" });
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Set cookie if requested
                if (request.RememberMe)
                {
                    var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(30)
                    };
                    Response.Cookies.Append("auth_token", token, cookieOptions);
                }

                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        role = user.Role,
                        company = user.Company
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در سرور" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return Ok(new { message = "خروج موفقیت‌آمیز" });
        }

        private bool ValidateCaptcha(string captchaToken)
        {
            // Stub implementation - always return true for now
            // In real implementation, validate against captcha service
            return true;
        }

        private UserModel ValidateUser(string username, string password, string company)
        {
            // Mock users - in real app, query from database
            var users = new List<UserModel>
            {
                new UserModel
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Company = "demo",
                    FirstName = "مدیر",
                    LastName = "سیستم",
                    Role = "admin",
                    IsActive = true
                },
                new UserModel
                {
                    Id = 2,
                    Username = "accountant",
                    Password = "acc123",
                    Company = "demo",
                    FirstName = "حسابدار",
                    LastName = "اصلی",
                    Role = "finance",
                    IsActive = true
                },
                new UserModel
                {
                    Id = 3,
                    Username = "sales",
                    Password = "sales123",
                    Company = "demo",
                    FirstName = "کارشناس",
                    LastName = "فروش",
                    Role = "sales",
                    IsActive = true
                },
                new UserModel
                {
                    Id = 4,
                    Username = "user",
                    Password = "user123",
                    Company = "demo",
                    FirstName = "کاربر",
                    LastName = "عادی",
                    Role = "user",
                    IsActive = true
                }
            };

            return users.Find(u => u.Username == username && u.Password == password && u.Company == company && u.IsActive);
        }

        private string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-long-enough-for-security");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("company", user.Company)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public bool RememberMe { get; set; }
        public string CaptchaToken { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}