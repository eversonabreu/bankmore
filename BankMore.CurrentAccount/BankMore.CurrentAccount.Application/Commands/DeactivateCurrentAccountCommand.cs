using MediatR;

namespace BankMore.CurrentAccount.Application.Commands;

public record DeactivateCurrentAccountCommand(
    long NumberAccount,
    string Password
) : IRequest<bool>;