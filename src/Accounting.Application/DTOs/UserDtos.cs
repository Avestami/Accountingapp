using System;
using System.ComponentModel.DataAnnotations;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public UserRole Role { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        public bool IsActive { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class SetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}