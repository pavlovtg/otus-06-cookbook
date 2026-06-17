using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipeRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    // ── GetRecipesPagedAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_ReturnsAllPersistedRecipes()
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
        var result = await readCtx.GetRecipesPagedAsync(1, 1000);

        Assert.Contains(result.Items, r => r.Id == r1.Id);
        Assert.Contains(result.Items, r => r.Id == r2.Id);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_ReturnsCorrectTotal()
    {
        await using (var writeCtx = _factory.Create())
        {
            for (var i = 1; i <= 5; i++)
                await writeCtx.CreateAsync(NewRecipe());
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 2);

        Assert.Equal(5, result.Total);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_SecondPage_ReturnsDistinctItems()
    {
        await using (var writeCtx = _factory.Create())
        {
            for (var i = 1; i <= 5; i++)
                await writeCtx.CreateAsync(NewRecipe());
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var page1 = await readCtx.GetRecipesPagedAsync(1, 2);
        var page2 = await readCtx.GetRecipesPagedAsync(2, 2);

        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(2, page2.Items.Count);

        var page1Ids = page1.Items.Select(r => r.Id).ToHashSet();
        Assert.All(page2.Items, r => Assert.DoesNotContain(r.Id, page1Ids));
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
            loaded.Update("Обновлённый заголовок", null, 60, Difficulty.Everyday, 4, "Шаг 1.", isPublic: true);
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

    // ── Ingredients (8.4) ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithIngredients_GetByIdAsync_ReturnsIngredients()
    {
        var ingredientId = IngredientId.New();
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(Ingredient.Create(ingredientId, "Морковь", "г", 100f, IngredientCategory.Vegetables));
            await writeCtx.CommitAsync();
        }

        var recipe = Recipe.Create(
            RecipeId.New(), "Рецепт с ингредиентами", "Описание",
            45, Difficulty.Everyday, 4, "Шаг 1.",
            [RecipeIngredient.Create(ingredientId, 200m)]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(recipe);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Single(result.Ingredients);
        Assert.Equal(ingredientId, result.Ingredients[0].IngredientId);
        Assert.Equal(200m, result.Ingredients[0].Amount);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleIngredients_GetByIdAsync_ReturnsAllIngredients()
    {
        var id1 = IngredientId.New();
        var id2 = IngredientId.New();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(Ingredient.Create(id1, "Морковь", "г", 100f, IngredientCategory.Vegetables));
            await writeCtx.CreateAsync(Ingredient.Create(id2, "Лук", "шт.", 1f, IngredientCategory.Vegetables));
            await writeCtx.CommitAsync();
        }

        var recipe = Recipe.Create(
            RecipeId.New(), "Рецепт", "Описание",
            30, Difficulty.Easy, 2, "Шаг 1.",
            [RecipeIngredient.Create(id1, 150m), RecipeIngredient.Create(id2, 2m)]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(recipe);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Ingredients.Count);
    }

    // ── GetRecipesUsingIngredientAsync (8.5) ─────────────────────────────────

    [Fact]
    public async Task GetRecipesUsingIngredientAsync_WhenIngredientUsed_ReturnsTotalCount()
    {
        var ingredientId = IngredientId.New();
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(Ingredient.Create(ingredientId, "Картофель", "г", 200f, IngredientCategory.Vegetables));
            await writeCtx.CommitAsync();
        }

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(Recipe.Create(RecipeId.New(), "Рецепт 1", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.", [RecipeIngredient.Create(ingredientId, 300m)]));
            await writeCtx.CreateAsync(Recipe.Create(RecipeId.New(), "Рецепт 2", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.", [RecipeIngredient.Create(ingredientId, 100m)]));
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesUsingIngredientAsync(ingredientId);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TopTitles.Count);
    }

    [Fact]
    public async Task GetRecipesUsingIngredientAsync_WhenIngredientNotUsed_ReturnsZero()
    {
        await using var ctx = _factory.Create();
        var result = await ctx.GetRecipesUsingIngredientAsync(IngredientId.New());

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.TopTitles);
    }

    [Fact]
    public async Task GetRecipesUsingIngredientAsync_ReturnsTop10Titles()
    {
        var ingredientId = IngredientId.New();
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(Ingredient.Create(ingredientId, "Соль", "г", 5f, IngredientCategory.SpicesAndSeasonings));
            await writeCtx.CommitAsync();
        }

        await using (var writeCtx = _factory.Create())
        {
            for (var i = 1; i <= 12; i++)
            {
                await writeCtx.CreateAsync(Recipe.Create(
                    RecipeId.New(), $"Рецепт {i:D2}", "Описание",
                    30, Difficulty.Easy, 2, "Шаг 1.",
                    [RecipeIngredient.Create(ingredientId, 5m)]));
            }
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesUsingIngredientAsync(ingredientId);

        Assert.Equal(12, result.TotalCount);
        Assert.Equal(10, result.TopTitles.Count);
    }
}
