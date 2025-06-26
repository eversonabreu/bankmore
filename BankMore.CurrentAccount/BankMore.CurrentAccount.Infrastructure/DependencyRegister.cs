using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.CurrentAccount.Infrastructure;

public static class DependencyRegister
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentAccountRepository, CurrentAccountRepository>();
    }
}