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

public sealed class IdempotenceService(
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

        var idempotenceRequest = new IdempotenceRequestDto<TValue>(idempotenceKey, payloadRequest);

        return await ExecuteAsync(idempotenceRequest, messageTopicName, messageTopicHandler);
    }

    private async Task<(MovementOperationEnum Status, string PayloadResponse)> ExecuteAsync<TValue>(
        IdempotenceRequestDto<TValue> body,
        string messageTopicName,
        IMessageTopicHandler messageTopicHandler)
    {
        try
        {
            logger.LogInformation("Create the executor idempotence. Key: {Key}. Request payload: {Payload}", 
                body.IdempotenceKey, body.PayloadRequisition);

            var result = await messageTopicHandler.HandleAsync(body);
            return (MovementOperationEnum.Success, result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing or saving idempotence. Key: {Key}. Request payload: {Payload}",
                body.IdempotenceKey, body.PayloadRequisition);

            if (await CreateMessagingIdempotenceAsync(body, messageTopicName))
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
            await repository.SaveChangesAsync();

            logger.LogInformation("New idempotence entry created with key: {Key}", idempotenceKey);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while create idempotence. Key: {Key}", idempotenceKey);
        }

        return false;
    }

    private async Task<bool> CreateMessagingIdempotenceAsync<TValue>(IdempotenceRequestDto<TValue> body,
        string messageTopicName)
    {
        try
        {
            await messageService.PublishAsync(messageTopicName, body);

            logger.LogInformation("Message published to Kafka for retry. Topic: {Topic}, Key: {Key}", messageTopicName, body.IdempotenceKey);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish message to Kafka. Topic: {Topic}, Key: {Key}", messageTopicName, body.IdempotenceKey);
        }

        return false;
    }

    public async Task<string> SavePayloadResponseAsync(string idempotenceKey, object payload)
    {
        var idempotence = await repository.SingleAsync(x => x.Key == idempotenceKey);
        idempotence.PayloadResponse = JsonSerializer.Serialize(payload);
        await repository.UpdateAsync(idempotence);
        await repository.SaveChangesAsync();
        return idempotence.PayloadResponse;
    }
}