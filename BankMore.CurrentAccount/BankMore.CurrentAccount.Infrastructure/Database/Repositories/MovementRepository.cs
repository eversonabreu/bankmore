using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Repositories;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class MovementRepository(ApplicationDbContext context)
    : DbRepository<Domain.Entities.Movement, ApplicationDbContext>(context), IMovementRepository
{
}