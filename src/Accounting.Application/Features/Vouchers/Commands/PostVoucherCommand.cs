using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class PostVoucherCommand : ICommand<Result<VoucherDto>>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int PostedByUserId { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
    }
}