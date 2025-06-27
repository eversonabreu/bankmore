using BankMore.CurrentAccount.Domain.Entities;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

public sealed class IdempotenceService(
    IIdempotenceRepository repository,
    IMemoryCache cache,
    ILogger<IdempotenceService> logger,
    IConfiguration configuration) : IIdempotenceService
{
    public async Task<Idempotence> GetCompletedAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (cache.TryGetValue(key, out Idempotence cached) && !string.IsNullOrEmpty(cached.PayloadResponse))
        {
            logger.LogInformation("Idempotence hit from cache: {Key}", key);
            return cached;
        }

        var entity = await repository.SingleOrDefaultAsync(x => x.Key == key);

        if (entity is not null && !string.IsNullOrEmpty(entity.PayloadResponse))
        {
            logger.LogInformation("Idempotence found in DB and added to cache: {Key}", key);
            var expirationStr = configuration["Cache:ExpirationTime"];
            var minutes = int.TryParse(expirationStr, out var result) ? result : 10;
            var cacheExpiration = TimeSpan.FromMinutes(minutes);
            cache.Set(key, entity, cacheExpiration);
        }

        return entity;
    }
}