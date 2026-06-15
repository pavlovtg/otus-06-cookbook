using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

/// <summary>
/// Разделяемая фикстура для интеграционных тестов репозиториев.
/// Один PostgreSQL-контейнер + одна миграция на всю коллекцию.
/// </summary>
public sealed class RecipeIntegrationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var factory = new RepositoryFactory<RecipeRepository>(
            _postgres.GetConnectionString(),
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));

        await using (factory)
        {
            await factory.MigrateAsync();
        }
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    /// <summary>
    /// Очищает все пользовательские таблицы схемы cookbook между тестовыми классами.
    /// Таблица миграций EF исключается.
    /// </summary>
    public async Task TruncateAsync()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();

        await using var listCmd = conn.CreateCommand();
        listCmd.CommandText = """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = 'cookbook'
              AND table_type = 'BASE TABLE'
              AND table_name != '__EFMigrationsHistory'
            """;

        var tables = new List<string>();
        await using (var reader = await listCmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                tables.Add($"cookbook.\"{reader.GetString(0)}\"");
        }

        if (tables.Count == 0)
            return;

        await using var truncateCmd = conn.CreateCommand();
        truncateCmd.CommandText = $"TRUNCATE TABLE {string.Join(", ", tables)} RESTART IDENTITY CASCADE";
        await truncateCmd.ExecuteNonQueryAsync();
    }
}
