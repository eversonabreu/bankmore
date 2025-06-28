using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface ICurrentAccountRepository : IDbRepository<Entities.CurrentAccount>
{
    public Task<(Entities.CurrentAccount CurrentAccount, MovementOperationEnum? MovementOperation)>
        GetCurrentAccountByNumberAsync(long numberAccount);
}