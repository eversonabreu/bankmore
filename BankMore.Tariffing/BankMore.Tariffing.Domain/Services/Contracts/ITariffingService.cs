namespace BankMore.Tariffing.Domain.Services.Contracts;
public interface ITariffingService
{
    public Task CreateTariffingAsync(Guid transferId, Guid currentAccountOriginId);

    public Task<IReadOnlyCollection<Entities.Tariffing>> GetTariffingsAsync();
}