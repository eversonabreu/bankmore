using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.CurrentAccount.Domain;

public static class DependencyRegister
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
    }
}