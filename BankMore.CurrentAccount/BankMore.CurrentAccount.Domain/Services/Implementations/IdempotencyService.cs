using BankMore.CurrentAccount.Domain.Entities;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

public sealed class IdempotencyService(
    IIdempotencyRepository repository,
    IMemoryCache cache,
    ILogger<IdempotencyService> logger,
    IConfiguration configuration) : IIdempotencyService
{
    public async Task<Idempotency> GetCompletedAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (cache.TryGetValue(key, out Idempotency cached) && !string.IsNullOrEmpty(cached.PayloadResponse))
        {
            logger.LogInformation("Idempotency hit from cache: {Key}", key);
            return cached;
        }

        var entity = await repository.SingleOrDefaultAsync(x => x.Key == key);

        if (entity is not null && !string.IsNullOrEmpty(entity.PayloadResponse))
        {
            logger.LogInformation("Idempotency found in DB and added to cache: {Key}", key);
            var expirationStr = configuration["Cache:ExpirationTime"];
            var minutes = int.TryParse(expirationStr, out var result) ? result : 10;
            var cacheExpiration = TimeSpan.FromMinutes(minutes);
            cache.Set(key, entity, cacheExpiration);
        }

        return entity;
    }
}