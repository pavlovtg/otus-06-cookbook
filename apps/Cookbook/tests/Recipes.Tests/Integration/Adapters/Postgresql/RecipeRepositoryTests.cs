using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Recipes.Adapters.Postgresql;
using Recipes.Application.Ports;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class RecipeRepositoryTests : IAsyncLifetime
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

    private static Recipe NewRecipe(RecipeId? id = null) =>
        Recipe.Create(
            id ?? RecipeId.New(),
            "Тестовый рецепт",
            "Описание",
            30, Difficulty.Easy, 2,
            "Шаг 1. Готовить.");

    private static Recipe NewRecipeWithIngredients(IEnumerable<RecipeIngredient> ingredients, RecipeId? id = null) =>
        Recipe.Create(
            id ?? RecipeId.New(),
            "Рецепт с ингредиентами",
            "Описание",
            45, Difficulty.Everyday, 4,
            "Шаг 1. Готовить.",
            ingredients);

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
        await foreach (var r in readCtx.GetRecipesAsync())
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

    // ── Ingredients (8.4) ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithIngredients_GetByIdAsync_ReturnsIngredients()
    {
        var ingredientId = IngredientId.New();
        var ingredient = Ingredient.Create(ingredientId, "Морковь", "г", 100f, IngredientCategory.Vegetables);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(ingredient);
            await writeCtx.CommitAsync();
        }

        var recipeIngredient = RecipeIngredient.Create(ingredientId, 200m);
        var recipe = NewRecipeWithIngredients([recipeIngredient]);

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

        var recipe = NewRecipeWithIngredients([
            RecipeIngredient.Create(id1, 150m),
            RecipeIngredient.Create(id2, 2m),
        ]);

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

        var ri = RecipeIngredient.Create(ingredientId, 300m);
        var r1 = NewRecipeWithIngredients([ri]);
        var r2 = NewRecipeWithIngredients([RecipeIngredient.Create(ingredientId, 100m)]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(r1);
            await writeCtx.CreateAsync(r2);
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
        var ingredientId = IngredientId.New();

        await using var ctx = _factory.Create();
        var result = await ctx.GetRecipesUsingIngredientAsync(ingredientId);

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
                var recipe = Recipe.Create(
                    RecipeId.New(), $"Рецепт {i:D2}", "Описание",
                    30, Difficulty.Easy, 2, "Шаг 1.",
                    [RecipeIngredient.Create(ingredientId, 5m)]);
                await writeCtx.CreateAsync(recipe);
            }
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesUsingIngredientAsync(ingredientId);

        Assert.Equal(12, result.TotalCount);
        Assert.Equal(10, result.TopTitles.Count);
    }

    // ── RecipePhoto (TEST-4) ─────────────────────────────────────────────────

    private static RecipePhoto NewPhoto(RecipeId recipeId) =>
        RecipePhoto.Create(
            RecipePhotoId.New(),
            recipeId,
            "image/jpeg",
            [0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02, 0x03],
            [0xFF, 0xD8, 0xFF, 0xE0, 0x04, 0x05]);

    [Fact]
    public async Task SaveAsync_ThenGetOriginalAsync_ReturnsOriginalData()
    {
        var recipe = NewRecipe();
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        var photo = NewPhoto(recipe.Id);
        await using (var ctx = _factory.Create())
        {
            await ((IRecipePhotoRepository)ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var original = await ((IRecipePhotoRepository)readCtx).GetOriginalAsync(photo.Id);

        Assert.NotNull(original);
        Assert.Equal(photo.OriginalData, original);
    }

    [Fact]
    public async Task GetOriginalAsync_DoesNotReturnThumbnailData_AndGetThumbnailAsync_ReturnsCorrectData()
    {
        var recipe = NewRecipe();
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        var photo = NewPhoto(recipe.Id);
        await using (var ctx = _factory.Create())
        {
            await ((IRecipePhotoRepository)ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var original = await ((IRecipePhotoRepository)readCtx).GetOriginalAsync(photo.Id);
        var thumbnail = await ((IRecipePhotoRepository)readCtx).GetThumbnailAsync(photo.Id);

        Assert.NotNull(original);
        Assert.NotNull(thumbnail);
        Assert.Equal(photo.OriginalData, original);
        Assert.Equal(photo.ThumbnailData, thumbnail);
        Assert.NotEqual(original, thumbnail);
    }

    [Fact]
    public async Task GetOriginalAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await ((IRecipePhotoRepository)ctx).GetOriginalAsync(RecipePhotoId.New());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetThumbnailAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await ((IRecipePhotoRepository)ctx).GetThumbnailAsync(RecipePhotoId.New());
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRecipe_CascadeDeletesPhoto()
    {
        var recipe = NewRecipe();
        await using (var ctx = _factory.Create())
        {
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        var photo = NewPhoto(recipe.Id);
        await using (var ctx = _factory.Create())
        {
            await ((IRecipePhotoRepository)ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.DeleteAsync(recipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await ((IRecipePhotoRepository)readCtx).GetOriginalAsync(photo.Id);
        Assert.Null(result);
    }
}
