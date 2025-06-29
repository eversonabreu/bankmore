using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IMovementService
{
    public Task<(MovementOperationEnum Status, string PayloadResponse)>
    GetOrSaveMovementAsync(string idempotenceKey,
        long? requestNumberAccount, MovementTypeEnum movementType, decimal value, long loggedNumberAccount);

    public Task<(BalanceDto Balance, MovementOperationEnum Status)> GetBalanceAsync(long numberAccount);

    public Task<(bool IsSuccess, string MessageError)> TransferAsync(long numberAccountOrigin, long numberAccountDestination, decimal value);
}