using BankMore.Core.Infrastructure.Messaging;
using BankMore.Tariffing.Domain.Services.Contracts;
using BankMore.Tariffing.Domain.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Tariffing.Domain;

public static class DependencyRegister
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ITariffingService, TariffingService>();
        services.AddKeyedScoped<IMessageTopicHandler, TransferMessageService>(Topics.CurrentAccountTransferTopicName);
    }
}