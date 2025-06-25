using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Helpers;

namespace BankMore.CurrentAccount.Infrastructure.Database.Migrations.Configuration;

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