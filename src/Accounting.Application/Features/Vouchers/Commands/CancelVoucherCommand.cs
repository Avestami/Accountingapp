using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class CancelVoucherCommand : ICommand<Result<VoucherDto>>
    {
        [Required]
        public int Id { get; set; }
        
        public string? Reason { get; set; }
    }
}