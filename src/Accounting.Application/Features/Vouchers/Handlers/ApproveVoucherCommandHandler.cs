using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Models;
using Accounting.Domain.Enums;
using AutoMapper;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class ApproveVoucherCommandHandler : ICommandHandler<ApproveVoucherCommand, Result<VoucherDto>>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly ILogger<ApproveVoucherCommandHandler> _logger;

        public ApproveVoucherCommandHandler(
            IVoucherRepository voucherRepository,
            ILogger<ApproveVoucherCommandHandler> logger)
        {
            _voucherRepository = voucherRepository;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(ApproveVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(request.Id);
                if (voucher == null)
                {
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                // Check if voucher can be approved
                if (voucher.Status != VoucherStatus.Pending)
                {
                    return Result.Failure<VoucherDto>("Only pending vouchers can be approved");
                }

                // Approve the voucher
                voucher.Approve();
                if (!string.IsNullOrEmpty(request.Notes))
                {
                    voucher.Notes = request.Notes;
                }

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

                _logger.LogInformation("Voucher {VoucherId} approved successfully", request.Id);
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>($"Error approving voucher: {ex.Message}");
            }
        }
    }
}