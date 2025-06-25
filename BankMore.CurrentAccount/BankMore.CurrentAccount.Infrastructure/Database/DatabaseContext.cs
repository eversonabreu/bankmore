using BankMore.Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankMore.CurrentAccount.Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<DatabaseContext> options) : DatabaseContext(options)
{
}