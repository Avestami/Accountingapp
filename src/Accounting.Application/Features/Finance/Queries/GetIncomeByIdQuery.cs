using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Finance.Queries
{
    public class GetIncomeByIdQuery : IQuery<Result<IncomeDto>>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;
    }
}