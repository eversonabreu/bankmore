using Microsoft.EntityFrameworkCore;

namespace BankMore.Core.Infrastructure.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
}