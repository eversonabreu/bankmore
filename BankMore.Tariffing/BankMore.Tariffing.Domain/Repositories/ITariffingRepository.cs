using BankMore.Core.Infrastructure.Database;

namespace BankMore.Tariffing.Domain.Repositories;

public interface ITariffingRepository : IDbRepository<Entities.Tariffing>
{
}