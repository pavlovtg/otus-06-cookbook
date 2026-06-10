using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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

    private RecipeRepository CreateDbContext() =>
        new(new DbContextOptionsBuilder<RecipeRepository>()
            .UseNpgsql(
                _postgres.GetConnectionString(),
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema))
            .Options);

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
        Assert.Equal(RecipeRepository.DefaultSchema, schema);
    }
}
