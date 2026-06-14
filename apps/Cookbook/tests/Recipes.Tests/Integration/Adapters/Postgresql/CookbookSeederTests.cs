using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class CookbookSeederTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
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

    [Fact]
    public async Task SeedAsync_EmptyDb_InsertsAllIngredients()
    {
        await using var seedCtx = _factory.Create();
        await CookbookSeeder.SeedAsync(seedCtx);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Ingredients.CountAsync();

        Assert.Equal(SeedData.Ingredients.Length, count);
    }

    [Fact]
    public async Task SeedAsync_EmptyDb_InsertsAllRecipes()
    {
        await using var seedCtx = _factory.Create();
        await CookbookSeeder.SeedAsync(seedCtx);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Recipes.CountAsync();

        Assert.Equal(SeedData.Recipes.Length, count);
    }

    [Fact]
    public async Task SeedAsync_EmptyDb_InsertsRecipeIngredients()
    {
        await using var seedCtx = _factory.Create();
        await CookbookSeeder.SeedAsync(seedCtx);

        await using var readCtx = _factory.Create();
        var borscht = await readCtx.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == RecipeId.From(new Guid("11111111-0000-0000-0000-000000000001")));

        Assert.NotNull(borscht);
        Assert.NotEmpty(borscht.Ingredients);
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicate()
    {
        await using (var ctx1 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx1);

        await using (var ctx2 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var ingredientCount = await readCtx.Ingredients.CountAsync();
        var recipeCount = await readCtx.Recipes.CountAsync();

        Assert.Equal(SeedData.Ingredients.Length, ingredientCount);
        Assert.Equal(SeedData.Recipes.Length, recipeCount);
    }

    [Fact]
    public async Task SeedAsync_ExistingIngredient_UpdatesIt()
    {
        await using (var ctx1 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx1);

        var seedIngredient = SeedData.Ingredients[0];
        await using (var modifyCtx = _factory.Create())
        {
            var ingredient = await modifyCtx.Ingredients.FindAsync([seedIngredient.Id]);
            Assert.NotNull(ingredient);
            ingredient.Update("Изменённое название", "шт.", 1f, IngredientCategory.Other);
            await modifyCtx.SaveChangesAsync();
        }

        await using (var ctx2 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var result = await readCtx.Ingredients.FindAsync([seedIngredient.Id]);

        Assert.NotNull(result);
        Assert.Equal(seedIngredient.Title, result.Title);
        Assert.Equal(seedIngredient.Unit, result.Unit);
        Assert.Equal(seedIngredient.DefaultAmount, result.DefaultAmount);
        Assert.Equal(seedIngredient.Category, result.Category);
    }

    [Fact]
    public async Task SeedAsync_ExistingRecipe_UpdatesIt()
    {
        await using (var ctx1 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx1);

        var seedRecipe = SeedData.Recipes[0];
        await using (var modifyCtx = _factory.Create())
        {
            var recipe = await modifyCtx.Recipes.FindAsync([seedRecipe.Id]);
            Assert.NotNull(recipe);
            recipe.Update("Изменённый заголовок", null, 10, Difficulty.Easy, 1, "Шаг 1.");
            await modifyCtx.SaveChangesAsync();
        }

        await using (var ctx2 = _factory.Create())
            await CookbookSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var result = await readCtx.Recipes.FindAsync([seedRecipe.Id]);

        Assert.NotNull(result);
        Assert.Equal(seedRecipe.Title, result.Title);
    }

    [Fact]
    public async Task SeedAsync_RecipeIngredients_BorschtHasExpectedCount()
    {
        await using var seedCtx = _factory.Create();
        await CookbookSeeder.SeedAsync(seedCtx);

        var borschtId = RecipeId.From(new Guid("11111111-0000-0000-0000-000000000001"));
        var expectedCount = SeedData.Recipes
            .First(r => r.Id == borschtId)
            .Ingredients.Count;

        await using var readCtx = _factory.Create();
        var borscht = await readCtx.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == borschtId);

        Assert.NotNull(borscht);
        Assert.Equal(expectedCount, borscht.Ingredients.Count);
    }
}
