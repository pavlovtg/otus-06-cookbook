using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class CategoryRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
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

    private static Category NewCategory() =>
        Category.Create(CategoryId.New(), "Тестовая категория", "Описание.", CategoryType.MealRole);

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ThenCommit_CategoryIsPersisted()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.Type, result.Type);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await ctx.GetByIdAsync(CategoryId.New());

        Assert.Null(result);
    }

    // ── GetCategoriesAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetCategoriesAsync_ReturnsAllPersistedCategories()
    {
        var c1 = NewCategory();
        var c2 = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(c1);
            await ctx.CreateAsync(c2);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetCategoriesAsync();

        Assert.Contains(result, c => c.Id == c1.Id);
        Assert.Contains(result, c => c.Id == c2.Id);
    }

    // ── CountAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var c1 = NewCategory();
        var c2 = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(c1);
            await ctx.CreateAsync(c2);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var count = await readCtx.CountAsync();

        Assert.True(count >= 2);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ThenCommit_ChangesArePersisted()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            var loaded = await ctx.GetByIdAsync(category.Id);
            Assert.NotNull(loaded);
            loaded.Update("Обновлённая категория", "Новое описание.");
            await ctx.UpdateAsync(loaded);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal("Обновлённая категория", result.Name);
        Assert.Equal("Новое описание.", result.Description);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ThenCommit_CategoryIsRemoved()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.DeleteAsync(category.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(category.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        await using var ctx = _factory.Create();
        var exception = await Record.ExceptionAsync(() => ctx.DeleteAsync(CategoryId.New()));

        Assert.Null(exception);
    }

    // ── IsUsedInRecipesAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task IsUsedInRecipesAsync_NewCategory_ReturnsFalse()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var isUsed = await readCtx.IsUsedInRecipesAsync(category.Id);

        Assert.False(isUsed);
    }

    // ── Migration ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MigrateAsync_CategoriesTable_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*) FROM information_schema.tables
            WHERE table_schema = 'cookbook' AND table_name = 'categories'
            """;

        var count = (long?)await cmd.ExecuteScalarAsync();

        Assert.Equal(1L, count);
    }
}
