using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Repositories;
using System.Text.Json;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class TariffingMessageService(IMovementRepository movementRepository)
    : IMessageTopicHandler
{
    public async Task<string> HandleAsync(object body)
    {
        var tariffing = (TariffingDto)body;

        await movementRepository.CreateAsync(new Entities.Movement
        {
            CurrentAccountId = tariffing.Id,
            MovementDate = DateTime.UtcNow,
            MovementType = Enums.MovementTypeEnum.Debit,
            Value = tariffing.RateValue
        });

        await movementRepository.SaveChangesAsync();

        return tariffing.Id.ToString();
    }

    public async Task HandleMessageAsync(string payload)
        => await HandleAsync(JsonSerializer.Deserialize<TariffingDto>(payload));
}