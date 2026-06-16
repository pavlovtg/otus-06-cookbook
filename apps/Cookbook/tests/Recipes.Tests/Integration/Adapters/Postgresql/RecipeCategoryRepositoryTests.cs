using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class RecipeCategoryRepositoryTests : IAsyncLifetime
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

    private static Category NewCategory(CategoryType type = CategoryType.MealRole) =>
        Category.Create(CategoryId.New(), "Тестовая категория", "Описание", type);

    private static Recipe NewRecipe(RecipeId? id = null) =>
        Recipe.Create(
            id ?? RecipeId.New(),
            "Тестовый рецепт",
            "Описание",
            30, Difficulty.Easy, 2,
            "Шаг 1. Готовить.");

    [Fact]
    public async Task CreateRecipe_WithCategories_GetByIdAsync_ReturnsCategories()
    {
        var category = NewCategory(CategoryType.MealRole);

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        var recipeId = RecipeId.New();
        var categoryTypes = new Dictionary<CategoryId, CategoryType>
        {
            [category.Id] = CategoryType.MealRole,
        };
        var recipe = Recipe.Create(recipeId, "Рецепт", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
            categoryTypes: categoryTypes);

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipeId);

        Assert.NotNull(result);
        Assert.Single(result.Categories);
        Assert.Equal(category.Id, result.Categories[0].CategoryId);
    }

    [Fact]
    public async Task UpdateRecipe_ChangesCategories_GetByIdAsync_ReturnsUpdatedCategories()
    {
        var cat1 = NewCategory(CategoryType.MealRole);
        var cat2 = NewCategory(CategoryType.Cuisine);

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(cat1);
            await ctx.CreateAsync(cat2);
            await ctx.CommitAsync();
        }

        var recipeId = RecipeId.New();
        var recipe = Recipe.Create(recipeId, "Рецепт", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
            categoryTypes: new Dictionary<CategoryId, CategoryType> { [cat1.Id] = CategoryType.MealRole });

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            var loaded = await ctx.GetByIdAsync(recipeId);
            Assert.NotNull(loaded);
            loaded.Update("Рецепт", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
                categoryTypes: new Dictionary<CategoryId, CategoryType> { [cat2.Id] = CategoryType.Cuisine });
            await ctx.UpdateAsync(loaded);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetByIdAsync(recipeId);

        Assert.NotNull(result);
        Assert.Single(result.Categories);
        Assert.Equal(cat2.Id, result.Categories[0].CategoryId);
    }

    [Fact]
    public async Task IsUsedInRecipesAsync_WhenCategoryUsed_ReturnsTrue()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        var recipe = Recipe.Create(RecipeId.New(), "Рецепт", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
            categoryTypes: new Dictionary<CategoryId, CategoryType> { [category.Id] = CategoryType.MealRole });

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.IsUsedInRecipesAsync(category.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task IsUsedInRecipesAsync_WhenCategoryNotUsed_ReturnsFalse()
    {
        var category = NewCategory();

        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(category);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.IsUsedInRecipesAsync(category.Id);

        Assert.False(result);
    }
}
