using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

/// <summary>
/// Воспроизводит баг: MigrateAsync дважды на одной БД падает с 42P07
/// если __EFMigrationsHistory хранится в другой схеме чем ожидается.
/// </summary>
public sealed class MigrateIdempotencyTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    private RecipesDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<RecipesDbContext>()
            .UseNpgsql(
                _postgres.GetConnectionString(),
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", "cookbook"))
            .Options);

    [Fact]
    public async Task MigrateAsync_CalledTwiceSequentially_DoesNotThrow()
    {
        // Первый запуск — мигрирует БД
        await using var db1 = CreateDbContext();
        await db1.Database.MigrateAsync();

        // Второй запуск на той же БД — должен быть идемпотентным.
        // Без исправления упадёт с: 42P07 relation "recipes" already exists
        // потому что __EFMigrationsHistory пишется в public-схему,
        // а таблица создаётся в cookbook-схеме.
        await using var db2 = CreateDbContext();
        var ex = await Record.ExceptionAsync(() => db2.Database.MigrateAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task MigrateAsync_HistoryTable_IsInCookbookSchema()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        // Проверяем в какой схеме находится __EFMigrationsHistory
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT table_schema
            FROM information_schema.tables
            WHERE table_name = '__EFMigrationsHistory'
            """;

        var schema = (string?)await cmd.ExecuteScalarAsync();

        // Должна быть в схеме cookbook, а не public
        Assert.Equal("cookbook", schema);
    }
}
