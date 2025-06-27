using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class IdempotencyRepository(ApplicationDbContext context)
    : DbRepository<Domain.Entities.Idempotency, ApplicationDbContext>(context), IIdempotencyRepository
{
}