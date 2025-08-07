using System;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Vouchers.Queries
{
    public class GetVouchersQuery : IQuery<Result<PagedResult<VoucherDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public VoucherType? Type { get; set; }
        public VoucherStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Currency { get; set; }
        public int? TicketId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string SortBy { get; set; } = "VoucherDate";
        public bool SortDescending { get; set; } = true;
    }
}