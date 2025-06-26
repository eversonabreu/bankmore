using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class CurrentAccountRepository(ApplicationDbContext context) 
    : DbRepository<Domain.Entities.CurrentAccount, ApplicationDbContext>(context), ICurrentAccountRepository
{
}