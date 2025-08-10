using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class UpdateTransferStatusCommandHandler : ICommandHandler<UpdateTransferStatusCommand, Result<TransferDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IMapper _mapper;

        public UpdateTransferStatusCommandHandler(IAccountingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<TransferDto>> Handle(UpdateTransferStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transfer = await _context.Transfers
                    .Include(t => t.FromBankAccount)
                    .Include(t => t.ToBankAccount)
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.Company == request.Company, cancellationToken);

                if (transfer == null)
                {
                    return Result.Failure<TransferDto>("Transfer not found");
                }

                // Validate status transition
                if (transfer.Status == TransferStatus.Completed || transfer.Status == TransferStatus.Cancelled)
                {
                    return Result.Failure<TransferDto>("Cannot update status of completed or cancelled transfer");
                }

                if ((TransferStatus)request.Status == TransferStatus.Pending)
                {
                    return Result.Failure<TransferDto>("Cannot set status back to pending");
                }

                // Update transfer status
                transfer.Status = (TransferStatus)request.Status;
                transfer.UpdatedAt = System.DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(request.Notes))
                {
                    transfer.Notes = request.Notes;
                }

                await _context.SaveChangesAsync(cancellationToken);

                var transferDto = _mapper.Map<TransferDto>(transfer);
                return Result<TransferDto>.Success(transferDto);
            }
            catch (System.Exception ex)
            {
                return Result.Failure<TransferDto>($"Error updating transfer status: {ex.Message}");
            }
        }
    }
}