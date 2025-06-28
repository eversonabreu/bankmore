using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface IMovementRepository : IDbRepository<Entities.Movement>
{
    public Task<MovementOperationEnum> CreateMovementAsync(long numberAccount, MovementTypeEnum movementType, decimal value);
}