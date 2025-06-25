using Microsoft.EntityFrameworkCore;

namespace BankMore.Core.Infrastructure.Database;

public class DatabaseContext<TContext>(DbContextOptions<TContext> options) : DbContext(options) where TContext : DbContext
{
}