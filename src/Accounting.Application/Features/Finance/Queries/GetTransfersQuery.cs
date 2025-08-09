using System;
using MediatR;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Finance.Queries
{
    public class GetTransfersQuery : IRequest<Result<PagedResult<TransferDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Currency { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public string? SearchTerm { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}