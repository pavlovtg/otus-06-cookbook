using DotNet.Testcontainers.Builders;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Microservice;

/// <summary>
/// Разделяемая фикстура для микросервисных тестов.
/// Один PostgreSQL-контейнер + один WebApplicationFactory на всю коллекцию.
/// </summary>
public sealed class RecipeMicroserviceFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    private RecipeMicroserviceHost? _host;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _host = new RecipeMicroserviceHost(_postgres.GetConnectionString()).EnsureServer();
        Client = _host.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_host is not null)
            await _host.DisposeAsync();

        await _postgres.DisposeAsync();
    }

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
