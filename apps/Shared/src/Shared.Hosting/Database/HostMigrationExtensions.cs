using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Hosting.Database;

public static class HostMigrationExtensions
{
    public static async Task<IHost> MigrateDatabaseAsync<TProgram, TContext>(this IHost host)
        where TContext : DbContext
    {
        await using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TProgram>>();
        logger.LogInformation("Migrating database...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrated");
        return host;
    }
}
