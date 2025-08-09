using System;
using FluentValidation;
using Accounting.Application.Features.Finance.Commands;

namespace Accounting.Application.Features.Finance.Validators
{
    public class CreateIncomeCommandValidator : AbstractValidator<CreateIncomeCommand>
    {
        public CreateIncomeCommandValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.Now.AddDays(1))
                .WithMessage("Date cannot be in the future");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThan(1000000000)
                .WithMessage("Amount cannot exceed 1 billion");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)");

            RuleFor(x => x.PaymentSource)
                .NotEmpty()
                .WithMessage("Payment source is required");

            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessage("Company is required")
                .MaximumLength(50)
                .WithMessage("Company cannot exceed 50 characters");

            RuleFor(x => x.ExchangeRate)
                .GreaterThan(0)
                .WithMessage("Exchange rate must be greater than zero")
                .When(x => x.ExchangeRate.HasValue);

            RuleFor(x => x.Reference)
                .MaximumLength(100)
                .WithMessage("Reference cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Reference));

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}