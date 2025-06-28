using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class CurrentAccountRepository(ApplicationDbContext context) 
    : DbRepository<Domain.Entities.CurrentAccount, ApplicationDbContext>(context), ICurrentAccountRepository
{
    public async Task<(Domain.Entities.CurrentAccount CurrentAccount, MovementOperationEnum? MovementOperation)>
        GetCurrentAccountByNumberAsync(long numberAccount)
    {
        var currentAccount = await SingleOrDefaultAsync(x => x.Number == numberAccount);

        if (currentAccount is null)
            return (null, MovementOperationEnum.InvalidAccount);

        if (!currentAccount.IsActive)
            return (null, MovementOperationEnum.InactiveAccount);

        return (currentAccount, null);
    }
}