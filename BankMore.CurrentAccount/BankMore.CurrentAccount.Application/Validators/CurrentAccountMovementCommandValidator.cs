using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Helpers;
using FluentValidation;

namespace BankMore.CurrentAccount.Application.Validators;

public sealed class CurrentAccountMovementCommandValidator : AbstractValidator<CurrentAccountMovementCommand>
{
    public CurrentAccountMovementCommandValidator()
    {
        RuleFor(x => x.NumberAccount)
            .Must(number => number is null || number > 0)
            .WithMessage("If provided, the account number must be greater than 0.");

        RuleFor(x => x.Value)
            .GreaterThan(0m)
            .WithMessage($"{Constants.ApplicationErrors.FailMovementValue}|Value must be greater than 0.");

        RuleFor(x => x.MovementType)
            .IsInEnum()
            .WithMessage($"{Constants.ApplicationErrors.FailMovementType}|Invalid movement type.")
            .Must(type => type == MovementTypeEnum.Credit || type == MovementTypeEnum.Debit)
            .WithMessage($"{Constants.ApplicationErrors.FailMovementType}|MovementType must be either 'C' (Credit) or 'D' (Debit).");
    }
}