using BankMore.Transfer.Domain.Repositories;
using BankMore.Transfer.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Transfer.Infrastructure;

public static class DependencyRegister
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITransferRepository, TransfertRepository>();
    }
}