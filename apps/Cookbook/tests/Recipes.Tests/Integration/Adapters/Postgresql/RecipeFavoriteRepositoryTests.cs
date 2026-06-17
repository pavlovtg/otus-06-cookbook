using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipeFavoriteRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static User NewUser(UserId? id = null) =>
        User.Create(
            id ?? UserId.New(),
            $"user-{Guid.NewGuid():N}@test.local",
            "Test User",
            "hash",
            UserRole.User);

    // ── AddFavoriteAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task AddFavoriteAsync_ThenGetFavoriteIds_ContainsRecipeId()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var favoriteIds = await readCtx.GetFavoriteIdsAsync(user.Id);

        Assert.Contains(recipe.Id, favoriteIds);
    }

    [Fact]
    public async Task AddFavoriteAsync_CalledTwice_IsIdempotent()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        // Второй вызов не должен бросать исключение
        var exception = await Record.ExceptionAsync(async () =>
        {
            await using var ctx = _factory.Create();
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        });

        Assert.Null(exception);

        await using var readCtx = _factory.Create();
        var favoriteIds = await readCtx.GetFavoriteIdsAsync(user.Id);
        Assert.Single(favoriteIds);
    }

    // ── RemoveFavoriteAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task RemoveFavoriteAsync_AfterAdd_RecipeIdNotInFavorites()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.RemoveFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var favoriteIds = await readCtx.GetFavoriteIdsAsync(user.Id);

        Assert.DoesNotContain(recipe.Id, favoriteIds);
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WhenNotExists_IsIdempotent()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        // Удаляем несуществующую запись — не должно бросать
        var exception = await Record.ExceptionAsync(async () =>
        {
            await using var ctx = _factory.Create();
            await ctx.RemoveFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        });

        Assert.Null(exception);
    }

    // ── GetRecipesPagedAsync с favorites=true ─────────────────────────────────

    [Fact]
    public async Task GetRecipesPagedAsync_WithFavoritesTrue_ReturnsOnlyFavorites()
    {
        var user = NewUser();
        var favoriteRecipe = NewRecipe();
        var otherRecipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(favoriteRecipe);
            await ctx.CreateAsync(otherRecipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, favoriteRecipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(
            page: 1, pageSize: 100,
            currentUserId: user.Id,
            favorites: true);

        Assert.Single(result.Items);
        Assert.Equal(favoriteRecipe.Id, result.Items[0].Recipe.Id);
    }

    // ── Каскадное удаление ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRecipe_CascadesUserFavorites()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        // Удаляем рецепт — запись в user_favorites должна удалиться каскадно
        await using (var ctx = _factory.Create())
        {
            await ctx.DeleteAsync(recipe.Id);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var favoriteIds = await readCtx.GetFavoriteIdsAsync(user.Id);

        Assert.Empty(favoriteIds);
    }

    [Fact]
    public async Task DeleteUser_CascadesUserFavorites()
    {
        var user = NewUser();
        var recipe = NewRecipe();

        await using (var ctx = _factory.Create())
        {
            await ctx.Users.AddAsync(user);
            await ctx.CreateAsync(recipe);
            await ctx.CommitAsync();
        }

        await using (var ctx = _factory.Create())
        {
            await ctx.AddFavoriteAsync(user.Id, recipe.Id);
            await ctx.CommitAsync();
        }

        // Удаляем пользователя — его избранное должно удалиться каскадно
        await using (var ctx = _factory.Create())
        {
            var dbUser = await ctx.Users.FindAsync(user.Id);
            if (dbUser is not null)
                ctx.Users.Remove(dbUser);
            await ctx.CommitAsync();
        }

        // Рецепт остался, но избранного для удалённого пользователя нет
        await using var readCtx = _factory.Create();
        var favoriteIds = await readCtx.GetFavoriteIdsAsync(user.Id);

        Assert.Empty(favoriteIds);
    }
}
