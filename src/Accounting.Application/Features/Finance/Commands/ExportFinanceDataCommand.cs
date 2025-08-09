using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Finance.Commands
{
    public enum ExportFormat
    {
        Excel,
        Csv,
        Pdf
    }

    public enum ExportType
    {
        Costs,
        Incomes,
        Transfers,
        All
    }

    public class ExportFinanceDataCommand : IRequest<Result<byte[]>>
    {
        [Required]
        public ExportFormat Format { get; set; }

        [Required]
        public ExportType Type { get; set; }

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