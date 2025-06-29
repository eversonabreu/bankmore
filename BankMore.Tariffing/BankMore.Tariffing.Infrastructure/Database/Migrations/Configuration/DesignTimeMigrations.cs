using BankMore.Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using BankMore.Tariffing.Domain.Constants;

namespace BankMore.Tariffing.Infrastructure.Database.Migrations.Configuration;

public sealed class DesignTimeMigrations : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string connectionString = SQLiteConfiguration.GetConnectionString(Constants.ApplicationDatabaseName);
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}