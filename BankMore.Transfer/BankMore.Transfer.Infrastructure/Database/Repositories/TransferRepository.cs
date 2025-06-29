using BankMore.Core.Infrastructure.Database;
using BankMore.Transfer.Domain.Repositories;

namespace BankMore.Transfer.Infrastructure.Database.Repositories;

internal sealed class TransfertRepository(ApplicationDbContext context)
    : DbRepository<Domain.Entities.Transfer, ApplicationDbContext>(context), ITransferRepository
{
}