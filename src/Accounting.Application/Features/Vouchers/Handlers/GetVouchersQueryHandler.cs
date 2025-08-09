using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Accounting.Application.Common.Models;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Vouchers.Queries;
using Microsoft.Extensions.Logging;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class GetVouchersQueryHandler : IQueryHandler<GetVouchersQuery, Result<PagedResult<VoucherDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetVouchersQueryHandler> _logger;

        public GetVouchersQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetVouchersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<VoucherDto>>> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var vouchers = await _unitOfWork.Vouchers
                    .GetPagedAsync(
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        includes: v => v.Entries,
                        orderBy: v => v.VoucherDate,
                        ascending: false
                    );

                var voucherDtos = _mapper.Map<IEnumerable<VoucherDto>>(vouchers.Items);
                
                var totalPages = (int)Math.Ceiling((double)vouchers.TotalCount / request.PageSize);
                
                var result = new PagedResult<VoucherDto>(
                    voucherDtos,
                    vouchers.TotalCount,
                    request.PageNumber,
                    request.PageSize
                );

                _logger.LogInformation("Retrieved {Count} vouchers (page {PageNumber} of {TotalPages})", 
                    vouchers.Items.Count(), request.PageNumber, totalPages);
                
                return Result.Success<PagedResult<VoucherDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vouchers for page {PageNumber}", request.PageNumber);
                return Result.Failure<PagedResult<VoucherDto>>("An error occurred while retrieving vouchers");
            }
        }
    }
}