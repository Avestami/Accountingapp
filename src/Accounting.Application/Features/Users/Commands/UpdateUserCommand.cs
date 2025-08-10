using System.ComponentModel.DataAnnotations;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Users.Commands
{
    public class UpdateUserCommand : ICommand<Result<UserDto>>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
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
}