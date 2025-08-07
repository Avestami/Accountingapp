using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Vouchers.Queries;
using Accounting.Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class GetVoucherByIdQueryHandler : IQueryHandler<GetVoucherByIdQuery, Result<VoucherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetVoucherByIdQueryHandler> _logger;

        public GetVoucherByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetVoucherByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(GetVoucherByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers
                    .GetByIdWithIncludesAsync(request.Id, v => v.Entries, v => v.Entries.Select(e => e.Account));

                if (voucher == null)
                {
                    _logger.LogWarning("Voucher with ID {VoucherId} not found", request.Id);
                    return Result.Failure<VoucherDto>("Voucher not found");
                }

                var voucherDto = _mapper.Map<VoucherDto>(voucher);
                
                _logger.LogInformation("Retrieved voucher {VoucherNumber} successfully", voucher.VoucherNumber);
                
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voucher {VoucherId}", request.Id);
                return Result.Failure<VoucherDto>("An error occurred while retrieving the voucher");
            }
        }
    }
}