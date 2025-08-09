using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;
using Accounting.Application.Common.Models;
using Accounting.Domain.Enums;
using AutoMapper;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class RejectVoucherCommandHandler : ICommandHandler<RejectVoucherCommand, Result<VoucherDto>>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly ILogger<RejectVoucherCommandHandler> _logger;

        public RejectVoucherCommandHandler(
            IVoucherRepository voucherRepository,
            ILogger<RejectVoucherCommandHandler> logger)
        {
            _voucherRepository = voucherRepository;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(RejectVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(request.Id);
                if (voucher == null)
                {
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                // Check if voucher can be rejected
                if (voucher.Status != VoucherStatus.Pending)
                {
                    return Result.Failure<VoucherDto>("Only pending vouchers can be rejected");
                }

                // Reject the voucher
                voucher.Reject();
                voucher.Notes = request.Reason;

                await _voucherRepository.UpdateAsync(voucher);
                await _voucherRepository.SaveChangesAsync();

                // Map to DTO
                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    VoucherNumber = voucher.VoucherNumber,
                    Type = voucher.Type,
                    TypeName = voucher.Type.ToString(),
                    Date = voucher.VoucherDate,
                    Description = voucher.Description,
                    Reference = voucher.Reference,
                    Status = voucher.Status,
                    StatusName = voucher.Status.ToString(),
                    IsPosted = voucher.IsPosted,
                    PostedDate = voucher.PostedDate,
                    PostedByUserId = voucher.PostedByUserId,
                    TicketId = voucher.TicketId,
                    CreatedByUserId = voucher.CreatedByUserId,
                    CreatedAt = voucher.CreatedAt,
                    UpdatedAt = voucher.UpdatedAt ?? DateTime.UtcNow
                };

                _logger.LogInformation("Voucher {VoucherId} rejected successfully", request.Id);
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>($"Error rejecting voucher: {ex.Message}");
            }
        }
    }
}