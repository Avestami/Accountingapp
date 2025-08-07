using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Vouchers.Queries
{
    public class GetVoucherByIdQuery : IQuery<Result<VoucherDto>>
    {
        [Required]
        public int Id { get; set; }
        
        public GetVoucherByIdQuery(int id)
        {
            Id = id;
        }
    }
}