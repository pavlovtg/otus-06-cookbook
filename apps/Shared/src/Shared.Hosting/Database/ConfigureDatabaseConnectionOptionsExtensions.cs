using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Database.ConnectionStrings;

namespace Shared.Hosting.Database;

public static class ConfigureDatabaseConnectionOptionsExtensions
{
    public static IServiceCollection AddDatabaseConnectionOptions<TContext>(this IServiceCollection services, IConfiguration section) where TContext : DbContext
    {
        return services.Configure<DatabaseConnectionOptions<TContext>>(
            options =>
            {
                foreach (var child in section.GetChildren())
                {
                    options.Extensions[child.Key] = child.Get<object>();
                }
            });
    }
}
