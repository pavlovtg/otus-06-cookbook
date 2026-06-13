using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class RecipeRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    private RepositoryFactory<RecipeRepository> _factory = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new RepositoryFactory<RecipeRepository>(
            _postgres.GetConnectionString(),
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));

        await _factory.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static Recipe NewRecipe(RecipeId? id = null) =>
        Recipe.Create(
            id ?? RecipeId.New(),
            "Тестовый рецепт",
            "Описание",
            30, Difficulty.Easy, 2,
            "Шаг 1. Готовить.");

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ThenCommit_RecipeIsPersisted()
    {
        var recipe = NewRecipe();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(recipe);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal(recipe.Id, result.Id);
        Assert.Equal(recipe.Title, result.Title);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await ctx.GetByIdAsync(RecipeId.New());

        Assert.Null(result);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllPersistedRecipes()
    {
        var r1 = NewRecipe();
        var r2 = NewRecipe();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(r1);
            await writeCtx.CreateAsync(r2);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var all = new List<Recipe>();
        await foreach (var r in readCtx.GetAllAsync())
            all.Add(r);

        Assert.Contains(all, r => r.Id == r1.Id);
        Assert.Contains(all, r => r.Id == r2.Id);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ThenCommit_ChangesArePersisted()
    {
        var recipe = NewRecipe();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(recipe);
            await writeCtx.CommitAsync();
        }

        await using (var updateCtx = _factory.Create())
        {
            var loaded = await updateCtx.GetByIdAsync(recipe.Id);
            Assert.NotNull(loaded);
            loaded.Update("Обновлённый заголовок", null, 60, Difficulty.Everyday, 4, "Шаг 1.");
            await updateCtx.UpdateAsync(loaded);
            await updateCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal("Обновлённый заголовок", result.Title);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ThenCommit_RecipeIsRemoved()
    {
        var recipe = NewRecipe();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(recipe);
            await writeCtx.CommitAsync();
        }

        await using (var deleteCtx = _factory.Create())
        {
            await deleteCtx.DeleteAsync(recipe.Id);
            await deleteCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipe.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        await using var ctx = _factory.Create();
        var exception = await Record.ExceptionAsync(() => ctx.DeleteAsync(RecipeId.New()));

        Assert.Null(exception);
    }

    // ── Migration ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MigrateAsync_Schema_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
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
    public async Task MigrateAsync_SecondContext_CanQueryMigratedSchema()
    {
        // Новый инстанс контекста — отдельный DbContext, уже мигрированная БД
        await using var ctx = _factory.Create();

        var canConnect = await ctx.Database.CanConnectAsync();
        Assert.True(canConnect);

        // Убеждаемся что таблица recipes доступна для запросов
        var count = await ctx.Recipes.CountAsync();
        Assert.Equal(0, count);
    }
}
