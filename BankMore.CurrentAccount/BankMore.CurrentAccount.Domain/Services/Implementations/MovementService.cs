using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class MovementService(IIdempotenceService idempotenceService,
    [FromKeyedServices(Topics.CurrentAccountMovementTopicName)] IMessageTopicHandler messageTopicHandler,
    ICurrentAccountRepository currentAccountRepository,
    IMovementRepository movementRepository) : IMovementService
{
    public async Task<(MovementOperationEnum Status, string PayloadResponse)> 
        GetOrSaveMovementAsync(string idempotenceKey, 
            long? requestNumberAccount, MovementTypeEnum movementType, decimal value, long loggedNumberAccount)
    {
        if (string.IsNullOrWhiteSpace(idempotenceKey))
            return (MovementOperationEnum.IdempotenceKeyNullOrEmpty, null);

        var numberAccount = requestNumberAccount ?? loggedNumberAccount;

        if (numberAccount != loggedNumberAccount && movementType == MovementTypeEnum.Debit)
            return (MovementOperationEnum.InvalidType, null);

        var (_, movementOperation) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccount);

        if (movementOperation != null)
            return (movementOperation.Value, null);

        var movement = new MovementDto(numberAccount, movementType, value);

        var result = await idempotenceService
            .ProcessAsync(idempotenceKey, movement, Topics.CurrentAccountMovementTopicName, messageTopicHandler);

        if (result.PayloadResponse != null
            && result.PayloadResponse.Contains(Constants.ApplicationMessages.FatalErrorMovementCurrentAccountError))
            return (MovementOperationEnum.FatalErrorProccessing, null);

        return result;
    }

    public async Task<(BalanceDto Balance, MovementOperationEnum Status)> GetBalanceAsync(long numberAccount)
    {
        var (currentAccount, movementOperation) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccount);

        if (movementOperation != null)
            return (null, movementOperation.Value);

        var balanceValue = await movementRepository.GetBalanceCurrentAcount(numberAccount);

        return (new(numberAccount, currentAccount.Name, DateTime.UtcNow, balanceValue), MovementOperationEnum.Success);
    } 
}