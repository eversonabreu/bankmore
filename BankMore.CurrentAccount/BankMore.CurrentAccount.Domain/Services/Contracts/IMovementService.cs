using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IMovementService
{
    public Task<(MovementOperationEnum Status, string PayloadResponse)>
    GetOrSaveMovementAsync(string idempotenceKey,
        long? requestNumberAccount, MovementTypeEnum movementType, decimal value, long loggedNumberAccount);
}