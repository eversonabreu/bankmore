using BankMore.Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using BankMore.Transfer.Domain.Constants;

namespace BankMore.Transfer.Infrastructure.Database.Migrations.Configuration;

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