using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class RejectVoucherCommand : ICommand<Result<VoucherDto>>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Reason { get; set; }
    }
}