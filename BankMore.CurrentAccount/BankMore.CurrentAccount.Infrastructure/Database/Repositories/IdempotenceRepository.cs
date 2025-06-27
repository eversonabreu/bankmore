using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class IdempotenceRepository(ApplicationDbContext context)
    : DbRepository<Domain.Entities.Idempotence, ApplicationDbContext>(context), IIdempotenceRepository
{
}