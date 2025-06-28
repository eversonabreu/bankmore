using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class MovementService(IIdempotenceService idempotenceService,
    [FromKeyedServices(Topics.CurrentAccountMovementTopicName)] IMessageTopicHandler messageTopicHandler,
    ICurrentAccountRepository currentAccountRepository) : IMovementService
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

        return await idempotenceService
            .ProcessAsync(idempotenceKey, movement, Topics.CurrentAccountMovementTopicName, messageTopicHandler);
    }
}