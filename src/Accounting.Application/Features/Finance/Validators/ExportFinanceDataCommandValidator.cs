using System;
using FluentValidation;
using Accounting.Application.Features.Finance.Commands;

namespace Accounting.Application.Features.Finance.Validators
{
    public class ExportFinanceDataCommandValidator : AbstractValidator<ExportFinanceDataCommand>
    {
        public ExportFinanceDataCommandValidator()
        {
            RuleFor(x => x.Format)
                .IsInEnum()
                .WithMessage("Invalid export format");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid export type");

            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessage("Company is required")
                .MaximumLength(50)
                .WithMessage("Company cannot exceed 50 characters");

            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate)
                .WithMessage("From date must be before or equal to To date")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

            RuleFor(x => x.ToDate)
                .LessThanOrEqualTo(DateTime.Now.AddDays(1))
                .WithMessage("To date cannot be in the future")
                .When(x => x.ToDate.HasValue);

            RuleFor(x => x.Currency)
                .Length(3)
                .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)")
                .When(x => !string.IsNullOrEmpty(x.Currency));

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}