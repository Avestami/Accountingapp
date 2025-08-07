using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Accounting.Application.Features.Vouchers.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Models;
using Accounting.Domain.Enums;
using Accounting.Domain.Entities;
using AutoMapper;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Exceptions;

namespace Accounting.Application.Features.Vouchers.Handlers
{
    public class CreateVoucherCommandHandler : ICommandHandler<CreateVoucherCommand, Result<VoucherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateVoucherCommandHandler> _logger;

        public CreateVoucherCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateVoucherCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VoucherDto>> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate entries
                if (!request.Entries.Any())
                {
                    return Result.Failure<VoucherDto>("Voucher must have at least one entry");
                }

                // Validate balance
                var totalDebit = request.Entries.Sum(e => e.DebitAmount);
            var totalCredit = request.Entries.Sum(e => e.CreditAmount);
                
                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    return Result.Failure<VoucherDto>("Voucher entries must be balanced (total debits must equal total credits)");
                }

                // Validate ticket exists if provided
                if (request.TicketId.HasValue)
                {
                    var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId.Value);
                    if (ticket == null)
                    {
                        return Result.Failure<VoucherDto>("Ticket not found");
                    }
                }

                // Validate accounts exist
                var accountIds = request.Entries.Select(e => e.AccountId).Distinct().ToList();
                var accounts = await _unitOfWork.Accounts.FindAsync(a => accountIds.Contains(a.Id));
                if (accounts.Count() != accountIds.Count)
                {
                    return Result.Failure<VoucherDto>("One or more accounts not found");
                }

                // Generate voucher number
                var voucherNumber = await GenerateVoucherNumberAsync(request.Type);

                // Create voucher
                var voucher = new Voucher
                {
                    VoucherNumber = voucherNumber,
                    Type = request.Type,
                    Description = request.Description,
                    Currency = request.Currency,
                    VoucherDate = request.VoucherDate,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    TicketId = request.TicketId,
                    Amount = totalDebit, // or totalCredit, they should be equal
                    Status = VoucherStatus.Draft
                };

                // Create entries
                foreach (var entryDto in request.Entries)
                {
                    var entry = new VoucherEntry
                    {
                        VoucherId = voucher.Id,
                        AccountId = entryDto.AccountId,
                        Description = entryDto.Description,
                        Amount = entryDto.DebitAmount > 0 ? entryDto.DebitAmount : entryDto.CreditAmount,
                        TransactionType = entryDto.DebitAmount > 0 ? TransactionType.Debit : TransactionType.Credit,
                        Currency = entryDto.Currency ?? request.Currency
                    };
                    
                    voucher.Entries.Add(entry);
                }

                await _unitOfWork.Vouchers.AddAsync(voucher);
                await _unitOfWork.SaveChangesAsync();

                var voucherDto = _mapper.Map<VoucherDto>(voucher);
                
                _logger.LogInformation("Voucher {VoucherNumber} created successfully", voucherNumber);
                
                return Result.Success<VoucherDto>(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating voucher");
                return Result.Failure<VoucherDto>("An error occurred while creating the voucher");
            }
        }

        private async Task<string> GenerateVoucherNumberAsync(VoucherType type)
        {
            var prefix = type switch
            {
                VoucherType.Income => "IV",
                VoucherType.Expense => "EV",
                VoucherType.Transfer => "TV",
                _ => "GV"
            };

            var documentNumber = await _unitOfWork.DocumentNumbers
                .FirstOrDefaultAsync(d => d.DocumentType == $"Voucher_{type}");

            if (documentNumber == null)
            {
                // Create new document number configuration
                documentNumber = new DocumentNumber
                {
                    DocumentType = $"Voucher_{type}",
                    Prefix = prefix,
                    CurrentNumber = 1,
                    PadLength = 6
                };
                await _unitOfWork.DocumentNumbers.AddAsync(documentNumber);
            }
            else
            {
                documentNumber.CurrentNumber++;
                await _unitOfWork.DocumentNumbers.UpdateAsync(documentNumber);
            }

            return $"{documentNumber.Prefix}{documentNumber.CurrentNumber.ToString().PadLeft(documentNumber.PadLength, '0')}";
        }
    }
}