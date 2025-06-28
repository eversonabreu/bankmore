using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using MediatR;

namespace BankMore.CurrentAccount.Application.Handlers;

public sealed class CurrentAccountMovementHandler(IMovementRepository movementRepository,
    ICurrentAccountRepository currentAccountRepository) 
    : IRequestHandler<CurrentAccountMovementCommand, MovementOperationEnum>
{
    public async Task<MovementOperationEnum> Handle(CurrentAccountMovementCommand request, CancellationToken cancellationToken)
    {
        var (currentAccount, movementOperation) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(request.NumberAccount.Value);

        if (movementOperation != null)
            return movementOperation.Value;

        await movementRepository.CreateAsync(new Domain.Entities.Movement
        {
            MovementDate = DateTime.UtcNow,
            MovementType = request.MovementType,
            CurrentAccountId = currentAccount.Id,
            Value = request.Value
        });

        return MovementOperationEnum.Success;
    }
}