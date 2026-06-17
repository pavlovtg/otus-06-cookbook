using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Application;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipeRepositorySearchTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static Recipe NewRecipe(string title, string description = "Описание") =>
        Recipe.Create(RecipeId.New(), title, description, 30, Difficulty.Easy, 2, "Шаг 1.");

    // ── Фильтрация по title ──────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_FilterByTitle_ReturnsMatchingRecipes()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Борщ украинский"));
            await ctx.CreateAsync(NewRecipe("Пельмени сибирские"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "борщ");

        Assert.Single(result.Items);
        Assert.Equal("Борщ украинский", result.Items[0].Recipe.Title);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_FilterByTitle_CaseInsensitive()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Борщ"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "БОРЩ");

        Assert.Single(result.Items);
    }

    // ── Фильтрация по description ────────────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_FilterByDescription_ReturnsMatchingRecipes()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Рецепт 1", "Классический украинский суп"));
            await ctx.CreateAsync(NewRecipe("Рецепт 2", "Итальянская паста"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "украинский");

        Assert.Single(result.Items);
        Assert.Equal("Рецепт 1", result.Items[0].Recipe.Title);
    }

    // ── Фильтрация по category.name ──────────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_FilterByCategoryName_ReturnsMatchingRecipes()
    {
        var category = Category.Create(CategoryId.New(), "Завтрак", "Утренние блюда", CategoryType.MealTime);

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        var recipeWithCategory = Recipe.Create(
            RecipeId.New(), "Омлет", "Описание", 15, Difficulty.Easy, 1, "Шаг 1.",
            categoryTypes: new Dictionary<CategoryId, CategoryType> { [category.Id] = CategoryType.MealTime });

        var recipeWithout = NewRecipe("Борщ");

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipeWithCategory);
            await ctx.CreateAsync(recipeWithout);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "завтрак");

        Assert.Single(result.Items);
        Assert.Equal("Омлет", result.Items[0].Recipe.Title);
    }

    // ── Фильтрация по ingredient.title ───────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_FilterByIngredientTitle_ReturnsMatchingRecipes()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), "Картофель", "г", 200f, IngredientCategory.Vegetables);

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(ingredient);
            await ctx.CommitAsync();
        }

        var recipeWithIngredient = Recipe.Create(
            RecipeId.New(), "Картофельный суп", "Описание", 40, Difficulty.Easy, 4, "Шаг 1.",
            [RecipeIngredient.Create(ingredient.Id, 300m)]);

        var recipeWithout = NewRecipe("Борщ");

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipeWithIngredient);
            await ctx.CreateAsync(recipeWithout);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "картофель");

        Assert.Single(result.Items);
        Assert.Equal("Картофельный суп", result.Items[0].Recipe.Title);
    }

    // ── AND-логика по нескольким словам ──────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_MultiWordQuery_AppliesAndLogic()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Борщ украинский", "Классический"));
            await ctx.CreateAsync(NewRecipe("Борщ постный", "Без мяса"));
            await ctx.CreateAsync(NewRecipe("Суп украинский", "Другой"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "борщ украинский");

        Assert.Single(result.Items);
        Assert.Equal("Борщ украинский", result.Items[0].Recipe.Title);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_QueryNoMatch_ReturnsEmpty()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Борщ"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, q: "несуществующееслово99999");

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    // ── Сортировка ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_SortTitleAsc_ReturnsSortedAscending()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Щи"));
            await ctx.CreateAsync(NewRecipe("Борщ"));
            await ctx.CreateAsync(NewRecipe("Окрошка"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, sort: RecipeSortOrder.TitleAsc);

        var titles = result.Items.Select(r => r.Recipe.Title).ToList();
        Assert.Equal(titles.OrderBy(t => t).ToList(), titles);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_SortTitleDesc_ReturnsSortedDescending()
    {
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(NewRecipe("Щи"));
            await ctx.CreateAsync(NewRecipe("Борщ"));
            await ctx.CreateAsync(NewRecipe("Окрошка"));
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, sort: RecipeSortOrder.TitleDesc);

        var titles = result.Items.Select(r => r.Recipe.Title).ToList();
        Assert.Equal(titles.OrderByDescending(t => t).ToList(), titles);
    }
}
