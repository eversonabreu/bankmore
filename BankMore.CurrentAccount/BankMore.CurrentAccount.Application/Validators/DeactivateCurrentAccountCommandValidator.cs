using BankMore.CurrentAccount.Application.Commands;
using FluentValidation;

namespace BankMore.CurrentAccount.Application.Validators;

public sealed class DeactivateCurrentAccountCommandValidator : AbstractValidator<DeactivateCurrentAccountCommand>
{
    public DeactivateCurrentAccountCommandValidator()
    {
        RuleFor(x => x.NumberAccount)
            .Must(x => x > 0).WithMessage("O número da conta-corrente deve ser informado obrigatóriamente e o valor deve ser acima de zero.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha deve ser informada obrigatóriamente.");
    }
}