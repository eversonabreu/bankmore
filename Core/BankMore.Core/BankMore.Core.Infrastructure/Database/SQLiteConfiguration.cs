using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Core.Infrastructure.Database;

public static class SQLiteConfiguration
{
    public static IServiceCollection AddSQliteConfiguredDbContext<TContext>(
        this IServiceCollection services,
        string dbFileName)
        where TContext : DbContext
    {
        var connectionString = GetConnectionString(dbFileName);

        services.AddDbContext<TContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }

    public static string GetConnectionString(string dbFileName)
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current != null)
        {
            var candidatePath = Path.Combine(current.FullName, "SQliteDatabases");

            if (Directory.Exists(candidatePath))
            {
                string path = Path.Combine(candidatePath, dbFileName);
                return $"Data Source={path}";
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Folder 'SQliteDatabases' not found.");
    }
}