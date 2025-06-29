using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace BankMore.Core.Infrastructure.HttpWrapper;

public static class HttpResiliencePolice
{
    public static IAsyncPolicy<HttpResponseMessage> CreatePolicy(int maxAttempts = 3, ILogger logger = null)
    {
        var policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(maxAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, delay, retryCount, context) =>
                {
                    logger?.LogWarning("Retry {RetryCount} after {Delay}s", retryCount, delay.TotalSeconds);
                    return Task.CompletedTask;
                });

        return policy;
    }
}