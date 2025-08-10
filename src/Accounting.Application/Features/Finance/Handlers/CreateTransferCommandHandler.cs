using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Finance.Commands;
using Accounting.Application.Interfaces;
using Accounting.Application.Services;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Finance.Handlers
{
    public class CreateTransferCommandHandler : ICommandHandler<CreateTransferCommand, Result<TransferDto>>
    {
        private readonly IAccountingDbContext _context;
        private readonly IDocumentNumberService _documentNumberService;

        public CreateTransferCommandHandler(IAccountingDbContext context, IDocumentNumberService documentNumberService)
        {
            _context = context;
            _documentNumberService = documentNumberService;
        }

        public async Task<Result<TransferDto>> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate accounts exist and are different
                if (request.FromAccountId == request.ToAccountId)
                {
                    return Result.Failure<TransferDto>("From and To accounts cannot be the same");
                }

                var fromBankAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(a => a.Id == request.FromAccountId, cancellationToken);

                var toBankAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(a => a.Id == request.ToAccountId, cancellationToken);

                if (fromBankAccount == null)
                {
                    return Result.Failure<TransferDto>("From bank account not found");
                }

                if (toBankAccount == null)
                {
                    return Result.Failure<TransferDto>("To bank account not found");
                }

                // Generate document number
                var documentNumber = await _documentNumberService.GetNextNumberAsync("TRANSFER", request.Company);

                // Calculate local amount
                var localAmount = request.ExchangeRate.HasValue 
                    ? request.Amount * request.ExchangeRate.Value 
                    : request.Amount;

                // Create transfer entity
                var transfer = new Transfer
                {
                    DocumentNumber = documentNumber,
                    Date = request.Date,
                    Description = request.Description,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ExchangeRate = request.ExchangeRate,
                    FromBankAccountId = request.FromAccountId,
                    ToBankAccountId = request.ToAccountId,
                    Reference = request.Reference,
                    Notes = request.Notes,
                    Status = Accounting.Domain.Entities.TransferStatus.Draft,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync(cancellationToken);

                // Return DTO
                var transferDto = new TransferDto
                {
                    Id = transfer.Id,
                    DocumentNumber = transfer.DocumentNumber,
                    Date = transfer.Date,
                    Description = transfer.Description,
                    Amount = transfer.Amount,
                    Currency = transfer.Currency,
                    ExchangeRate = transfer.ExchangeRate,
                    FromAccountId = transfer.FromBankAccountId,
                    FromAccountName = fromBankAccount.AccountName,
                    ToAccountId = transfer.ToBankAccountId,
                    ToAccountName = toBankAccount.AccountName,
                    Reference = transfer.Reference,
                    Notes = transfer.Notes,
                    Status = transfer.Status,
                    Company = transfer.Company,
                    CreatedAt = transfer.CreatedAt,
                    UpdatedAt = transfer.UpdatedAt
                };

                return Result.Success(transferDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<TransferDto>($"Error creating transfer: {ex.Message}");
            }
        }
    }
}