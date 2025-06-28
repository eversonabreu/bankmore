using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class MovementRepository(ApplicationDbContext context, ICurrentAccountRepository currentAccountRepository)
    : DbRepository<Domain.Entities.Movement, ApplicationDbContext>(context), IMovementRepository
{
    public async Task<MovementOperationEnum> CreateMovementAsync(long numberAccount, MovementTypeEnum movementType, decimal value)
    {
        if (value <= 0m)
            return MovementOperationEnum.InvalidValue;

        var (currentAccount, movementOperation) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccount);

        if (movementOperation != null)
            return movementOperation.Value;

        await CreateAsync(new Domain.Entities.Movement
        {
            MovementDate = DateTime.UtcNow,
            MovementType = movementType,
            CurrentAccountId = currentAccount.Id,
            Value = value
        });

        return MovementOperationEnum.Success;
    }
}