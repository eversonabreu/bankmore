using BankMore.Core.Infrastructure.Database;
using BankMore.Tariffing.Domain.Repositories;

namespace BankMore.Tariffing.Infrastructure.Database.Repositories;

internal sealed class TariffingRepository(ApplicationDbContext context)
    : DbRepository<Domain.Entities.Tariffing, ApplicationDbContext>(context), ITariffingRepository
{
}