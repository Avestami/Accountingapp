using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;
using Accounting.Application.Features.Counterparties.Commands;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Features.Counterparties.Handlers
{
    public class CreateCounterpartyCommandHandler : ICommandHandler<CreateCounterpartyCommand, Result<CounterpartyDto>>
    {
        private readonly ApplicationDbContext _context;

        public CreateCounterpartyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CounterpartyDto>> Handle(CreateCounterpartyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Check if code already exists
                var existingCounterparty = await _context.Counterparties
                    .FirstOrDefaultAsync(c => c.Code == command.Code, cancellationToken);

                if (existingCounterparty != null)
                {
                    return Result<CounterpartyDto>.Failure("Counterparty code already exists");
                }

                // Validate that counterparty is either customer or supplier (or both)
                if (!command.IsCustomer && !command.IsSupplier)
                {
                    return Result<CounterpartyDto>.Failure("Counterparty must be either a customer or supplier");
                }

                var counterparty = new Counterparty
                {
                    Name = command.Name,
                    Code = command.Code,
                    TaxId = command.TaxId,
                    Address = command.Address,
                    Phone = command.Phone,
                    Email = command.Email,
                    ContactPerson = command.ContactPerson,
                    IsCustomer = command.IsCustomer,
                    IsSupplier = command.IsSupplier,
                    IsActive = command.IsActive,
                    OpeningBalanceIRR = command.OpeningBalanceIRR,
                    OpeningBalanceUSD = command.OpeningBalanceUSD,
                    OpeningBalanceEUR = command.OpeningBalanceEUR,
                    OpeningBalanceAED = command.OpeningBalanceAED,
                    CreditLimit = command.CreditLimit,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Counterparties.Add(counterparty);
                await _context.SaveChangesAsync(cancellationToken);

                var dto = MapToDto(counterparty);
                return Result<CounterpartyDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<CounterpartyDto>.Failure($"Error creating counterparty: {ex.Message}");
            }
        }

        private CounterpartyDto MapToDto(Counterparty counterparty)
        {
            return new CounterpartyDto
            {
                Id = counterparty.Id,
                Name = counterparty.Name,
                Code = counterparty.Code,
                TaxId = counterparty.TaxId,
                Address = counterparty.Address,
                Phone = counterparty.Phone,
                Email = counterparty.Email,
                ContactPerson = counterparty.ContactPerson,
                IsCustomer = counterparty.IsCustomer,
                IsSupplier = counterparty.IsSupplier,
                IsActive = counterparty.IsActive,
                OpeningBalanceIRR = counterparty.OpeningBalanceIRR,
                OpeningBalanceUSD = counterparty.OpeningBalanceUSD,
                OpeningBalanceEUR = counterparty.OpeningBalanceEUR,
                OpeningBalanceAED = counterparty.OpeningBalanceAED,
                CreditLimit = counterparty.CreditLimit,
                CreatedAt = counterparty.CreatedAt,
                ModifiedAt = counterparty.ModifiedAt,
                // Current balances will be calculated by the query handlers
                CurrentBalanceIRR = counterparty.OpeningBalanceIRR,
                CurrentBalanceUSD = counterparty.OpeningBalanceUSD,
                CurrentBalanceEUR = counterparty.OpeningBalanceEUR,
                CurrentBalanceAED = counterparty.OpeningBalanceAED,
                TicketCount = 0,
                VoucherCount = 0
            };
        }
    }
}