using BankMore.Transfer.Domain.Services.Contracts;
using BankMore.Transfer.Domain.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Transfer.Domain;

public static class DependencyRegister
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ITransferService, TransferService>();
    }
}