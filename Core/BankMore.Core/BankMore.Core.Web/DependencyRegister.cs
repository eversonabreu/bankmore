using BankMore.Core.Infrastructure.HttpWrapper;
using BankMore.Core.Web.Filters;
using BankMore.Core.Web.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Core.Web;

public static class DependencyRegister
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHostedService<KafkaConsumerHostedService>();

        services.AddControllers(options =>
        {
            options.Filters.Add<AuthorizationFilter>();
        });

        services.AddHttpClient("ResilientClient")
            .AddPolicyHandler(sp =>
            {
                // add addiotional configuration for Polly
                // estou usando com no máximo de 3 retentativas, apenas para fins demonstrativos
                return HttpResiliencePolice.CreatePolicy(3, null);
            });
    }
}