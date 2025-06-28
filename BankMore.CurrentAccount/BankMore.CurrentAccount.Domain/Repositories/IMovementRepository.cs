using BankMore.Core.Infrastructure.Database;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface IMovementRepository : IDbRepository<Entities.Movement>
{
}