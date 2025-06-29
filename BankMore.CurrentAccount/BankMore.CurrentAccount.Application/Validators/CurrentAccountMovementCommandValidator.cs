using BankMore.CurrentAccount.Application.Requests;
using BankMore.CurrentAccount.Domain.Helpers;
using FluentValidation;

namespace BankMore.CurrentAccount.Application.Validators;

public sealed class CurrentAccountMovementCommandValidator : AbstractValidator<MovementRequest>
{
    public CurrentAccountMovementCommandValidator()
    {
        RuleFor(x => x.NumberAccount)
            .Must(number => number is null || number > 0)
            .WithMessage("If provided, the account number must be greater than 0.");

        RuleFor(x => x.Value)
            .GreaterThan(0m)
            .WithMessage($"{Constants.ApplicationMessages.FailMovementValue}|Value must be greater than 0.");

        RuleFor(x => x.MovementType)
            .Must(type => !string.IsNullOrEmpty(type) && (type.ToUpper()[0] == 'C' || type.ToUpper()[0] == 'D'))
            .WithMessage($"{Constants.ApplicationMessages.FailMovementType}|MovementType must be either 'C' (Credit) or 'D' (Debit).");
    }
}