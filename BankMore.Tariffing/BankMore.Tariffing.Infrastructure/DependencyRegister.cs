using BankMore.Tariffing.Domain.Repositories;
using BankMore.Tariffing.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Tariffing.Infrastructure;

public static class DependencyRegister
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITariffingRepository, TariffingRepository>();
    }
}