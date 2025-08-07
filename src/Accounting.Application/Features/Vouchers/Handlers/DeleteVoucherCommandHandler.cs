using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class DeleteVoucherCommandHandler : ICommandHandler<DeleteVoucherCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteVoucherCommandHandler> _logger;

        public DeleteVoucherCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteVoucherCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers
                    .GetByIdWithIncludesAsync(request.Id, v => v.Entries);

                if (voucher == null)
                {
                    return Result.Failure<bool>("Voucher not found");
                }

                // Check if voucher can be deleted
                if (voucher.IsPosted)
                {
                    return Result.Failure<bool>("Cannot delete a posted voucher");
                }

                if (voucher.Status == VoucherStatus.Posted)
                {
                    return Result.Failure<bool>("Cannot delete a posted voucher");
                }

                if (voucher.Status == VoucherStatus.Approved)
                {
                    return Result.Failure<bool>("Cannot delete an approved voucher. Please reject it first.");
                }

                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();
                
                try
                {
                    // Delete voucher entries first (due to foreign key constraints)
                    foreach (var entry in voucher.Entries.ToList())
                    {
                        await _unitOfWork.VoucherEntries.DeleteAsync(entry.Id);
                    }

                    // Delete the voucher
                    await _unitOfWork.Vouchers.DeleteAsync(voucher.Id);
                    
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation("Voucher {VoucherNumber} deleted successfully", voucher.VoucherNumber);
                    
                    return Result.Success(true);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voucher {VoucherId}", request.Id);
                return Result.Failure<bool>("An error occurred while deleting the voucher");
            }
        }
    }
}