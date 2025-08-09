using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Finance.Queries
{
    public class GetIncomesQuery : IQuery<Result<PagedResult<IncomeDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Currency { get; set; }
        public int? CounterpartyId { get; set; }
        public string? SearchTerm { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;
    }
}