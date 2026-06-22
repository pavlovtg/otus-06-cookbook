using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class RecipeCommentRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static User NewUser(UserId? id = null) =>
        User.Create(
            id ?? UserId.New(),
            $"{Guid.NewGuid()}@test.local",
            "Test User",
            "hash",
            UserRole.User);

    private static Recipe NewRecipe(RecipeId? id = null) =>
        Recipe.Create(
            id ?? RecipeId.New(),
            "Тестовый рецепт",
            "Описание",
            30, Difficulty.Easy, 2,
            "Шаг 1.");

    private static RecipeComment NewComment(RecipeId recipeId, UserId authorId, string text = "Отличный рецепт!") =>
        RecipeComment.Create(RecipeCommentId.New(), recipeId, authorId, text);

    // ── AddCommentAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task AddCommentAsync_ThenCommit_CommentIsPersisted()
    {
        var userId = UserId.New();
        var recipeId = RecipeId.New();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.Users.AddAsync(NewUser(userId));
            await writeCtx.Recipes.AddAsync(NewRecipe(recipeId));
            await writeCtx.CommitAsync();
        }

        var comment = NewComment(recipeId, userId);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.AddCommentAsync(comment);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetCommentAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
        Assert.Equal(recipeId, result.RecipeId);
        Assert.Equal(userId, result.AuthorId);
        Assert.Equal(comment.Text, result.Text);
    }

    // ── GetCommentsPagedAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetCommentsPagedAsync_ReturnsCommentsSortedByCreatedAtDesc()
    {
        var userId1 = UserId.New();
        var userId2 = UserId.New();
        var userId3 = UserId.New();
        var recipeId = RecipeId.New();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.Users.AddAsync(NewUser(userId1));
            await writeCtx.Users.AddAsync(NewUser(userId2));
            await writeCtx.Users.AddAsync(NewUser(userId3));
            await writeCtx.Recipes.AddAsync(NewRecipe(recipeId));
            await writeCtx.CommitAsync();
        }

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.AddCommentAsync(NewComment(recipeId, userId1, "Первый"));
            await writeCtx.AddCommentAsync(NewComment(recipeId, userId2, "Второй"));
            await writeCtx.AddCommentAsync(NewComment(recipeId, userId3, "Третий"));
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetCommentsPagedAsync(recipeId, 1, 10);

        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.Items.Count);

        for (var i = 0; i < result.Items.Count - 1; i++)
            Assert.True(result.Items[i].CreatedAt >= result.Items[i + 1].CreatedAt);
    }

    [Fact]
    public async Task GetCommentsPagedAsync_EmptyRecipe_ReturnsEmptyList()
    {
        var recipeId = RecipeId.New();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.Recipes.AddAsync(NewRecipe(recipeId));
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetCommentsPagedAsync(recipeId, 1, 10);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }

    // ── Уникальный индекс (recipe_id, author_id) ─────────────────────────────

    [Fact]
    public async Task AddCommentAsync_DuplicateAuthorAndRecipe_ThrowsOnCommit()
    {
        var userId = UserId.New();
        var recipeId = RecipeId.New();

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.Users.AddAsync(NewUser(userId));
            await writeCtx.Recipes.AddAsync(NewRecipe(recipeId));
            await writeCtx.CommitAsync();
        }

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.AddCommentAsync(NewComment(recipeId, userId, "Первый комментарий"));
            await writeCtx.CommitAsync();
        }

        await using var writeCtx2 = _factory.Create();
        await writeCtx2.AddCommentAsync(NewComment(recipeId, userId, "Дублирующий комментарий"));

        await Assert.ThrowsAsync<DbUpdateException>(() => writeCtx2.CommitAsync());
    }
}
