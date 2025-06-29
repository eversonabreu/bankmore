using BankMore.Core.Infrastructure.Database;

namespace BankMore.Transfer.Domain.Repositories;

public interface ITransferRepository : IDbRepository<Entities.Transfer>
{
}