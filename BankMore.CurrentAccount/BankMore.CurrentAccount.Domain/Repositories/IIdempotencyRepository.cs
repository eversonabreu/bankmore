using BankMore.Core.Infrastructure.Database;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface IIdempotencyRepository : IDbRepository<Entities.Idempotency>
{
}