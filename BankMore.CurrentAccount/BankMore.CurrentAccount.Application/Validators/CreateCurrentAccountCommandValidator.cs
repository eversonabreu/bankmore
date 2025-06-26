using BankMore.Core.Utils.Validators;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Helpers;
using FluentValidation;

namespace BankMore.CurrentAccount.Application.Validators;

public sealed class CreateCurrentAccountCommandValidator : AbstractValidator<CreateCurrentAccountCommand>
{
    public CreateCurrentAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome deve ser informado obrigatóriamente.")
            .MaximumLength(150).WithMessage("O nome não pode ter mais que 150 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha deve ser informada obrigatóriamente.")
            .MaximumLength(255).WithMessage("A senha não pode ter mais que 255 caracteres.");

        RuleFor(x => x.PersonDocumentNumber)
            .NotEmpty().WithMessage($"{Constants.ApplicationErrors.FailSavePersonDocument}|O número do CPF deve ser informado obrigatóriamente.")
            .Must(x => x.CpfValidate()).WithMessage($"{Constants.ApplicationErrors.FailSavePersonDocument}|O CPF informado é inválido.");
    }
}