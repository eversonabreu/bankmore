using BankMore.Core.Infrastructure.Messaging;
using BankMore.Tariffing.Domain.Dtos;
using BankMore.Tariffing.Domain.Services.Contracts;
using System.Text.Json;

namespace BankMore.Tariffing.Domain.Services.Implementations;

internal sealed class TransferMessageService(ITariffingService tariffingService) : IMessageTopicHandler
{
    public async Task<string> HandleAsync(object body)
    {
        var transfer = (TransferDto)body;
        await tariffingService.CreateTariffingAsync(transfer.TransferId, transfer.OriginId);
        return transfer.TransferId.ToString();
    }

    public async Task HandleMessageAsync(string payload)
        => await HandleAsync(JsonSerializer.Deserialize<TransferDto>(payload));
}