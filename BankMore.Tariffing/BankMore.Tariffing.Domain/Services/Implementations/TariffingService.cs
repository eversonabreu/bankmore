using BankMore.Core.Infrastructure.Messaging;
using BankMore.Tariffing.Domain.Repositories;
using BankMore.Tariffing.Domain.Services.Contracts;
using Microsoft.Extensions.Logging;
using Cons = BankMore.Tariffing.Domain.Constants;

namespace BankMore.Tariffing.Domain.Services.Implementations;

public sealed class TariffingService(ITariffingRepository tariffingRepository,
    IMessageService messageService, ILogger<TariffingService> logger) : ITariffingService
{
    public async Task CreateTariffingAsync(Guid transferId, Guid currentAccountOriginId)
    {
        try
        {
            // the 'if' it's necessary to avoid double charging in case of a retry of consuming the message

            if (await tariffingRepository.FirstOrDefaultAsync(x => x.TransferId == transferId) is null)
            {
                logger.LogInformation("Create tariffing to current accound id: {CurrentAccountOriginId}.", currentAccountOriginId);

                await tariffingRepository.CreateAsync(new Entities.Tariffing
                {
                    DateTransaction = DateTime.UtcNow,
                    TransferId = transferId,
                    RateValue = Cons.Constants.RateValue
                });

                await tariffingRepository.SaveChangesAsync();
            }

            logger.LogInformation("Publish message tariffing to current accound id: {CurrentAccountOriginId}.", currentAccountOriginId);

            await messageService.PublishAsync(Topics.CurrentAccountTariffingTopicName, new { Id = currentAccountOriginId, Value = Cons.Constants.RateValue });
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Error when tariffing to current accound id: {CurrentAccountOriginId}.", currentAccountOriginId);
            throw;  // it's necessary because musen't be mark the message as consumed
        }
    }

    // simple get of the tariffings
    // aqui é só para listar no endpoint. É claro que poderia ter filtros, paginação...
    public async Task<IReadOnlyCollection<Entities.Tariffing>> GetTariffingsAsync()
        => await tariffingRepository.GetAsync(x => true);
}