using System.ComponentModel.DataAnnotations;
using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Finance.Commands
{
    public class UpdateTransferStatusCommand : ICommand<Result<TransferDto>>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public TransferStatus Status { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}