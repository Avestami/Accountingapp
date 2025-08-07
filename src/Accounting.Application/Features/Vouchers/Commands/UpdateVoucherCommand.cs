using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Vouchers.Commands
{
    public class UpdateVoucherCommand : ICommand<Result<VoucherDto>>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        public DateTime VoucherDate { get; set; }
        
        [StringLength(100)]
        public string Reference { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; }
        
        [Required]
        public List<UpdateVoucherEntryDto> Entries { get; set; } = new List<UpdateVoucherEntryDto>();
    }
}