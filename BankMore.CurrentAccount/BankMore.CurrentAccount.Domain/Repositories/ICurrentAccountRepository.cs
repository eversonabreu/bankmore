using BankMore.Core.Infrastructure.Database;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface ICurrentAccountRepository : IDbRepository<Entities.CurrentAccount>
{
}