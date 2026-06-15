using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class IngredientRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
{
    private RepositoryFactory<RecipeRepository> _factory = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();

        _factory = new RepositoryFactory<RecipeRepository>(
            fixture.ConnectionString,
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
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

}
