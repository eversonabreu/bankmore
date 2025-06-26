using MediatR;

namespace BankMore.CurrentAccount.Application.Commands;

public record CreateCurrentAccountCommand(
    string Name,
    string Password,
    string PersonDocumentNumber
) : IRequest<Domain.Entities.CurrentAccount>;