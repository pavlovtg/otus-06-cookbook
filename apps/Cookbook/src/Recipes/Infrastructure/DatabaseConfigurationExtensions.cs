using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Recipes.Adapters.Postgresql;
using Shared.Database.ConnectionStrings;
using Shared.Hosting.Database;

namespace Recipes.Infrastructure;

internal static class DatabaseConfigurationExtensions
{
    private const string DatabaseConnectionSectionName = "DatabaseConnection";

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseConnectionOptions<RecipeRepository>(
            configuration.GetSection(DatabaseConnectionSectionName));

        services.AddDbContext<RecipeRepository>(
            (provider, options) =>
            {
                options.UseNpgsql(
                    provider.GetRequiredService<IOptions<DatabaseConnectionOptions<RecipeRepository>>>().Value.ToConnectionString(),
                    o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));
            });

        return services;
    }
}
