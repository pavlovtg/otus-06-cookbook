using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

/// <summary>
/// Тесты миграций: каждый тест получает свежий контейнер через RecipeMigrationFixture.
/// </summary>
public sealed class RecipeMigrationTests : IAsyncLifetime
{
    private readonly RecipeMigrationFixture _fixture = new();
    private RepositoryFactory<RecipeRepository> _factory = null!;

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();

        _factory = new RepositoryFactory<RecipeRepository>(
            _fixture.ConnectionString,
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));

        await _factory.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task MigrateAsync_Schema_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*) FROM information_schema.schemata
            WHERE schema_name = 'cookbook'
            """;

        var count = (long?)await cmd.ExecuteScalarAsync();

        Assert.Equal(1L, count);
    }

    [Fact]
    public async Task MigrateAsync_CategoriesTable_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*) FROM information_schema.tables
            WHERE table_schema = 'cookbook' AND table_name = 'categories'
            """;

        var count = (long?)await cmd.ExecuteScalarAsync();

        Assert.Equal(1L, count);
    }

    [Fact]
    public async Task MigrateAsync_IngredientsTable_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*) FROM information_schema.tables
            WHERE table_schema = 'cookbook' AND table_name = 'ingredients'
            """;

        var count = (long?)await cmd.ExecuteScalarAsync();

        Assert.Equal(1L, count);
    }

    [Fact]
    public async Task MigrateAsync_WriteInOneContext_ReadInAnother_DataIsPersisted()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), "Морковь", "г", 100f, IngredientCategory.Vegetables);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredient.Id);

        Assert.NotNull(result);
        Assert.Equal(ingredient.Title, result.Title);
    }
}
