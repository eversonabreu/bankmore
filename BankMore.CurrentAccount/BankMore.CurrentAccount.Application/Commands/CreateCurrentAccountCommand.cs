using MediatR;

namespace BankMore.CurrentAccount.Application.Commands;

public record CreateCurrentAccountCommand(
    string Name,
    bool IsActive,
    string Password,
    string PersonDocumentNumber
) : IRequest<Domain.Entities.CurrentAccount>;