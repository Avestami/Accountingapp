using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class DeleteVoucherCommand : ICommand<Result<bool>>
    {
        [Required]
        public int Id { get; set; }
    }
}