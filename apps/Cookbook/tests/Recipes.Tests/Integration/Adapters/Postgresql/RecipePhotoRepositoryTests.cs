using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Application.Ports;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipePhotoRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static Recipe NewRecipe() =>
        Recipe.Create(RecipeId.New(), "Рецепт для фото", "Описание", 30, Difficulty.Easy, 2, "Шаг 1.");

    private static RecipePhoto NewPhoto(RecipeId recipeId) =>
        RecipePhoto.Create(
            RecipePhotoId.New(),
            recipeId,
            "image/jpeg",
            [0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02, 0x03],
            [0xFF, 0xD8, 0xFF, 0xE0, 0x04, 0x05]);

    private IRecipePhotoRepository PhotoRepo(RecipeRepository ctx) => (IRecipePhotoRepository)ctx;

    // ── SaveAsync / GetOriginalAsync ─────────────────────────────────────────

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
            await PhotoRepo(ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var original = await PhotoRepo(readCtx).GetOriginalAsync(photo.Id);

        Assert.NotNull(original);
        Assert.Equal(photo.OriginalData, original);
    }

    // ── GetThumbnailAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_ThenGetThumbnailAsync_ReturnsThumbnailData()
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
            await PhotoRepo(ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var thumbnail = await PhotoRepo(readCtx).GetThumbnailAsync(photo.Id);

        Assert.NotNull(thumbnail);
        Assert.Equal(photo.ThumbnailData, thumbnail);
    }

    [Fact]
    public async Task OriginalAndThumbnail_AreStoredSeparately()
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
            await PhotoRepo(ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var original = await PhotoRepo(readCtx).GetOriginalAsync(photo.Id);
        var thumbnail = await PhotoRepo(readCtx).GetThumbnailAsync(photo.Id);

        Assert.NotNull(original);
        Assert.NotNull(thumbnail);
        Assert.NotEqual(original, thumbnail);
    }

    // ── Not found ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOriginalAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await PhotoRepo(ctx).GetOriginalAsync(RecipePhotoId.New());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetThumbnailAsync_NonExistentId_ReturnsNull()
    {
        await using var ctx = _factory.Create();
        var result = await PhotoRepo(ctx).GetThumbnailAsync(RecipePhotoId.New());
        Assert.Null(result);
    }

    // ── Cascade delete ───────────────────────────────────────────────────────

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
            await PhotoRepo(ctx).SaveAsync(photo);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.DeleteAsync(recipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await PhotoRepo(readCtx).GetOriginalAsync(photo.Id);
        Assert.Null(result);
    }
}
