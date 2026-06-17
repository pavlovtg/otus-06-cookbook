using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipePrivacyRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static readonly UserId AuthorId = UserId.From(Guid.NewGuid());
    private static readonly UserId OtherId = UserId.From(Guid.NewGuid());

    private static Recipe NewPublicRecipe(string title = "Публичный рецепт") =>
        Recipe.Create(RecipeId.New(), title, "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
            isPublic: true, authorId: AuthorId);

    private static Recipe NewPrivateRecipe(string title = "Приватный рецепт") =>
        Recipe.Create(RecipeId.New(), title, "Описание", 30, Difficulty.Easy, 2, "Шаг 1.",
            isPublic: false, authorId: AuthorId);

    [Fact]
    public async Task GetRecipesPagedAsync_AnonymousUser_HidesPrivateRecipes()
    {
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(NewPublicRecipe());
            await writeCtx.CreateAsync(NewPrivateRecipe());
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, currentUserId: null);

        Assert.All(result.Items, r => Assert.True(r.Recipe.IsPublic));
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_AuthorUser_SeesOwnPrivateRecipes()
    {
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(NewPublicRecipe());
            await writeCtx.CreateAsync(NewPrivateRecipe());
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, currentUserId: AuthorId);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task GetRecipesPagedAsync_OtherUser_HidesPrivateRecipesOfOtherAuthor()
    {
        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.CreateAsync(NewPublicRecipe());
            await writeCtx.CreateAsync(NewPrivateRecipe());
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetRecipesPagedAsync(1, 100, currentUserId: OtherId);

        Assert.All(result.Items, r => Assert.True(r.Recipe.IsPublic));
        Assert.Equal(1, result.Total);
    }
}
