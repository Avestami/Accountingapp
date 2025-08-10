using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accounting.Application.Common.Models;
using Accounting.Application.Features.Banks.Commands;
using Accounting.Application.Interfaces;
using Accounting.Domain.Entities;

namespace Accounting.Application.Features.Banks.Handlers
{
    public class DeleteBankCommandHandler
    {
        private readonly IAccountingDbContext _context;
        private readonly ILogger<DeleteBankCommandHandler> _logger;

        public DeleteBankCommandHandler(IAccountingDbContext context, ILogger<DeleteBankCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bank = await _context.Banks.FindAsync(request.Id);
                if (bank == null)
                {
                    return Result.Failure<bool>("Bank not found");
                }

                // Check if bank has associated bank accounts
                var hasBankAccounts = await _context.BankAccounts.AnyAsync(ba => ba.BankId == request.Id, cancellationToken);
                if (hasBankAccounts)
                {
                    return Result.Failure<bool>("Cannot delete bank with associated bank accounts");
                }

                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bank with ID {BankId}", request.Id);
                return Result.Failure<bool>("An error occurred while deleting the bank");
            }
        }
    }
}