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

public sealed class IngredientRepositoryTests : IAsyncLifetime
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

    private static Ingredient NewIngredient(IngredientId? id = null) =>
        Ingredient.Create(
            id ?? IngredientId.New(),
            "Тестовый ингредиент",
            "г",
            100f,
            IngredientCategory.Vegetables);

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ThenCommit_IngredientIsPersisted()
    {
        var ingredient = NewIngredient();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(ingredient.Id);

        Assert.NotNull(result);
        Assert.Equal(ingredient.Id, result.Id);
        Assert.Equal(ingredient.Title, result.Title);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await ctx.GetByIdAsync(IngredientId.New());

        Assert.Null(result);
    }

    // ── GetIngredientsAsync (paged) ──────────────────────────────────────────

    [Fact]
    public async Task GetIngredientsAsync_ReturnsAllPersistedIngredients()
    {
        var i1 = NewIngredient();
        var i2 = NewIngredient();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(i1);
            await writeCtx.CreateAsync(i2);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 1, pageSize: 100);

        Assert.Contains(result.Items, i => i.Id == i1.Id);
        Assert.Contains(result.Items, i => i.Id == i2.Id);
    }

    [Fact]
    public async Task GetIngredientsAsync_WithTitleFilter_ReturnsFiltered()
    {
        var morkov = Ingredient.Create(IngredientId.New(), "Морковь", "г", 100f, IngredientCategory.Vegetables);
        var kartofel = Ingredient.Create(IngredientId.New(), "Картофель", "г", 200f, IngredientCategory.Vegetables);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(morkov);
            await writeCtx.CreateAsync(kartofel);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 1, pageSize: 100, titleFilter: "морк");

        Assert.Contains(result.Items, i => i.Id == morkov.Id);
        Assert.DoesNotContain(result.Items, i => i.Id == kartofel.Id);
    }

    [Fact]
    public async Task GetIngredientsAsync_WithCategoryFilter_ReturnsFiltered()
    {
        var vegetable = Ingredient.Create(IngredientId.New(), "Свёкла", "г", 150f, IngredientCategory.Vegetables);
        var meat = Ingredient.Create(IngredientId.New(), "Говядина", "г", 300f, IngredientCategory.MeatAndPoultry);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(vegetable);
            await writeCtx.CreateAsync(meat);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 1, pageSize: 100, categoryFilter: IngredientCategory.Vegetables);

        Assert.Contains(result.Items, i => i.Id == vegetable.Id);
        Assert.DoesNotContain(result.Items, i => i.Id == meat.Id);
    }

    [Fact]
    public async Task GetIngredientsAsync_ReturnsCorrectTotalAndSlice()
    {
        // Создаём 5 ингредиентов
        var ingredients = Enumerable.Range(1, 5)
            .Select(n => Ingredient.Create(IngredientId.New(), $"Ингредиент {n:D2}", "г", 100f, IngredientCategory.Vegetables))
            .ToList();

        await using (var writeCtx = _factory.Create())
        {
            foreach (var i in ingredients)
                await writeCtx.CreateAsync(i);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 2, pageSize: 2, categoryFilter: IngredientCategory.Vegetables);

        Assert.Equal(5, result.Total);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetIngredientsAsync_LastPage_ReturnsRemainingItems()
    {
        var ingredients = Enumerable.Range(1, 3)
            .Select(n => Ingredient.Create(IngredientId.New(), $"Остаток {n:D2}", "г", 100f, IngredientCategory.Legumes))
            .ToList();

        await using (var writeCtx = _factory.Create())
        {
            foreach (var i in ingredients)
                await writeCtx.CreateAsync(i);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 2, pageSize: 2, categoryFilter: IngredientCategory.Legumes);

        Assert.Equal(3, result.Total);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetIngredientsAsync_PageBeyondData_ReturnsEmptyItems()
    {
        var ingredient = NewIngredient();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetIngredientsAsync(page: 100, pageSize: 100);

        Assert.Empty(result.Items);
        Assert.True(result.Total >= 1);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ThenCommit_ChangesArePersisted()
    {
        var ingredient = NewIngredient();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        await using (var updateCtx = _factory.Create())
        {
            var loaded = await updateCtx.GetByIdAsync(ingredient.Id);
            Assert.NotNull(loaded);
            loaded.Update("Обновлённый ингредиент", "шт.", 5f, IngredientCategory.FruitsAndBerries);
            await updateCtx.UpdateAsync(loaded);
            await updateCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(ingredient.Id);

        Assert.NotNull(result);
        Assert.Equal("Обновлённый ингредиент", result.Title);
        Assert.Equal(IngredientCategory.FruitsAndBerries, result.Category);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ThenCommit_IngredientIsRemoved()
    {
        var ingredient = NewIngredient();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        await using (var deleteCtx = _factory.Create())
        {
            await deleteCtx.DeleteAsync(ingredient.Id);
            await deleteCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(ingredient.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        await using var ctx = _factory.Create();
        var exception = await Record.ExceptionAsync(() => ctx.DeleteAsync(IngredientId.New()));

        Assert.Null(exception);
    }

    // ── Migration ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MigrateAsync_IngredientsTable_IsCreatedInCookbookSchema()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
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
    public async Task MigrateAsync_SecondContext_CanQueryIngredientsTable()
    {
        await using var ctx = _factory.Create();

        var canConnect = await ctx.Database.CanConnectAsync();
        Assert.True(canConnect);

        var count = await ctx.Ingredients.CountAsync();
        Assert.Equal(0, count);
    }
}
