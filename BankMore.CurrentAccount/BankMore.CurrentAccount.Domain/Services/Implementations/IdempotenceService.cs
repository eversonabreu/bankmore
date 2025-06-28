using BankMore.Core.Infrastructure.Messaging;
using BankMore.Core.Utils.Extensions;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Entities;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class IdempotenceService(
    IIdempotenceRepository repository,
    IMemoryCache cache,
    ILogger<IdempotenceService> logger,
    IConfiguration configuration,
    IMessageService messageService) : IIdempotenceService
{
    private const int CacheDefaultTimeoutInMinutes = 10;

    public async Task<(MovementOperationEnum Status, string PayloadResponse)> ProcessAsync<TValue>(
        string idempotenceKey,
        TValue payloadRequest,
        string messageTopicName,
        IMessageTopicHandler messageTopicHandler)
    {
        var payload = JsonSerializer.Serialize(payloadRequest);
        var cachedIdempotence = await GetCachedIdempotenceAsync(idempotenceKey);

        if (cachedIdempotence != null)
        {
            if (cachedIdempotence.PayloadRequisition.GetDeterministicJson() != payload.GetDeterministicJson())
                return (MovementOperationEnum.IdempotenceRequestBodyMismatch, null);

            if (!string.IsNullOrEmpty(cachedIdempotence.PayloadResponse))
                return (MovementOperationEnum.Success, cachedIdempotence.PayloadResponse);

            return (MovementOperationEnum.WaitingFinishProccess, null);
        }

        if (!await CreateIdempotenceAsync(idempotenceKey, payload))
            return (MovementOperationEnum.FatalErrorProccessing, null);

        return await ExecuteAsync(idempotenceKey, payloadRequest, messageTopicName, messageTopicHandler, payload);
    }

    private async Task<(MovementOperationEnum Status, string PayloadResponse)> ExecuteAsync<TValue>(
        string idempotenceKey,
        TValue payloadRequest,
        string messageTopicName,
        IMessageTopicHandler messageTopicHandler,
        string jsonPayload)
    {
        try
        {
            logger.LogInformation("Create the executor idempotence. Key: {Key}. Request payload: {Payload}", idempotenceKey, jsonPayload);
            var result = await messageTopicHandler.HandleAsync(jsonPayload);
            return (MovementOperationEnum.Success, result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing or saving idempotence. Key: {Key}", idempotenceKey);

            if (await CreateMessagingIdempotenceAsync(idempotenceKey, payloadRequest, messageTopicName))
                return (MovementOperationEnum.WaitingFinishProccess, null);
        }

        return (MovementOperationEnum.FatalErrorProccessing, null);
    }

    private async Task<Idempotence> GetCachedIdempotenceAsync(string idempotenceKey)
    {
        if (cache.TryGetValue(idempotenceKey, out Idempotence cached) && !string.IsNullOrEmpty(cached?.PayloadResponse))
        {
            logger.LogInformation("Idempotence hit from cache: {Key}", idempotenceKey);
            return cached;
        }

        var entity = await repository.SingleOrDefaultAsync(x => x.Key == idempotenceKey);

        if (!string.IsNullOrEmpty(entity?.PayloadResponse))
        {
            var minutes = int.TryParse(configuration["Cache:ExpirationTime"], out var result) ? result : CacheDefaultTimeoutInMinutes;
            var cacheExpiration = TimeSpan.FromMinutes(minutes);

            cache.Set(idempotenceKey, entity, cacheExpiration);

            logger.LogInformation("Idempotence found in DB and added to cache: {Key}", idempotenceKey);
        }

        return entity;
    }

    private async Task<bool> CreateIdempotenceAsync(string idempotenceKey, string payload)
    {
        try
        {
            var entity = new Idempotence
            {
                Key = idempotenceKey,
                PayloadRequisition = payload
            };

            await repository.CreateAsync(entity);

            logger.LogInformation("New idempotence entry created with key: {Key}", idempotenceKey);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while create idempotence. Key: {Key}", idempotenceKey);
        }

        return false;
    }

    private async Task<bool> CreateMessagingIdempotenceAsync<TValue>(string idempotenceKey, TValue payloadRequest,
        string messageTopicName)
    {
        try
        {
            var idempotenceRequest = new IdempotenceRequestDto<TValue>(idempotenceKey, payloadRequest);
            await messageService.PublishAsync(messageTopicName, idempotenceRequest);

            logger.LogInformation("Message published to Kafka for retry. Topic: {Topic}, Key: {Key}", messageTopicName, idempotenceKey);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish message to Kafka. Topic: {Topic}, Key: {Key}", messageTopicName, idempotenceKey);
        }

        return false;
    }
}