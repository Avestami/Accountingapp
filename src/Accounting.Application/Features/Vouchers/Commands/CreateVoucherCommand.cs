using System;
using System.Collections.Generic;
using Accounting.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class CreateVoucherCommand : ICommand<Result<VoucherDto>>
    {
        [Required]
        public VoucherType Type { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
        
        public DateTime VoucherDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(100)]
        public string Reference { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; }
        
        public int? TicketId { get; set; }
        
        [Required]
        public List<CreateVoucherEntryDto> Entries { get; set; } = new List<CreateVoucherEntryDto>();
    }
}