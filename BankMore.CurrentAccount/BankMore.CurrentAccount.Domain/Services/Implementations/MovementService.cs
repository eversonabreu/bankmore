using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class MovementService(IIdempotenceService idempotenceService,
    [FromKeyedServices(Topics.CurrentAccountMovementTopicName)] IMessageTopicHandler messageTopicHandler,
    ICurrentAccountRepository currentAccountRepository,
    IMovementRepository movementRepository, IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IMovementService
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

    public async Task<(bool IsSuccess, string MessageError)> TransferAsync(long numberAccountOrigin, 
        long numberAccountDestination, decimal value, string authorizationHeader)
    {
        if (value <= 0m)
            return (false, "Value must be grather than 0.");

        if (numberAccountOrigin == numberAccountDestination)
            return (false, "It's not possible tranfer in the same account.");

        var (origin, movementOperationOrigin) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccountOrigin);

        if (movementOperationOrigin != null)
            return (false, "The current account origin is invalid or disabled.");

        var balanceValue = (await GetBalanceAsync(numberAccountOrigin)).Balance.BalanceValue;

        // não estou permitindo transferir mais do que tem disponível.
        // obviamente que se tivesse um limite de conta, aí seria considerado
        if (value > balanceValue)
            return (false, "The insuficient balance in current account for transaction.");

        var (destination, movementOperationDestination) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccountDestination);

        if (movementOperationDestination != null)
            return (false, "The current account destinatrion is invalid or disabled.");

        using var httpClient = httpClientFactory.CreateClient("ResilientClient");
        httpClient.BaseAddress = new Uri(configuration["TransferApiHost"]);
        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

        var response = await httpClient.PostAsJsonAsync("transfer/create", 
            new
            {
                CurrentAccountOriginId = origin.Id,
                CurrentAccountDestinationId = destination.Id,
                TransferValue = value
            });

        if (response.IsSuccessStatusCode)
        {
            await movementRepository.CreateAsync(new Entities.Movement
            {
                CurrentAccountId = origin.Id,
                MovementDate = DateTime.UtcNow,
                MovementType = MovementTypeEnum.Debit,
                Value = value
            });

            await movementRepository.CreateAsync(new Entities.Movement
            {
                CurrentAccountId = destination.Id,
                MovementDate = DateTime.UtcNow,
                MovementType = MovementTypeEnum.Credit,
                Value = value
            });

            await movementRepository.SaveChangesAsync();
            return (true, null);
        }
        
        string content = await response.Content.ReadAsStringAsync();
        return (false, $"Error in API Transfer. Content: '{content}'. Status code: '{(int)response.StatusCode}'.");
    }
}