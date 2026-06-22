using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipeCommentTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── GET /api/v1/recipes/{id}/comments ─────────────────────────────────────

    // 8.1 — публичный доступ
    [Fact]
    public async Task GetComments_PublicRecipe_Returns200WithoutAuth()
    {
        var recipeId = await CreateRecipeAsync();

        var response = await _client.GetAsync($"/api/v1/recipes/{recipeId}/comments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<CommentDto>>();
        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }

    // 8.1 — пагинация
    [Fact]
    public async Task GetComments_Pagination_ReturnsCorrectPage()
    {
        var recipeId = await CreateRecipeAsync();

        // Каждый комментарий от отдельного пользователя (уникальный индекс recipe_id + author_id)
        for (var i = 0; i < 5; i++)
        {
            var auth = await fixture.GetAuthHeaderAsync();
            await AddCommentAsync(recipeId, $"Комментарий {i + 1}", auth);
        }

        var response = await _client.GetAsync($"/api/v1/recipes/{recipeId}/comments?page=1&pageSize=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<CommentDto>>();
        Assert.NotNull(result);
        Assert.Equal(5, result.Total);
        Assert.Equal(3, result.Items.Count);
    }

    // 8.1 — пустой список
    [Fact]
    public async Task GetComments_NoComments_ReturnsEmptyList()
    {
        var recipeId = await CreateRecipeAsync();

        var result = await _client.GetFromJsonAsync<PagedResult<CommentDto>>(
            $"/api/v1/recipes/{recipeId}/comments");

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }

    // ── POST /api/v1/recipes/{id}/comments ────────────────────────────────────

    // 8.2 — успех
    [Fact]
    public async Task AddComment_Authorized_Returns201WithComment()
    {
        var recipeId = await CreateRecipeAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/comments");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(new { text = "Отличный рецепт!" });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<CommentDto>();
        Assert.NotNull(dto);
        Assert.Equal("Отличный рецепт!", dto.Text);
        Assert.Equal(recipeId, dto.RecipeId);
    }

    // 8.2 — без JWT → 401
    [Fact]
    public async Task AddComment_Unauthorized_Returns401()
    {
        var recipeId = await CreateRecipeAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/v1/recipes/{recipeId}/comments",
            new { text = "Без токена" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // 8.2 — превышение длины → 400
    [Fact]
    public async Task AddComment_TextTooLong_Returns400()
    {
        var recipeId = await CreateRecipeAsync();
        var longText = new string('А', 2001);

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/comments");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(new { text = longText });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── DELETE /api/v1/recipes/{id}/comments/{commentId} ─────────────────────

    // 8.3 — автор комментария может удалить
    [Fact]
    public async Task DeleteComment_ByAuthor_Returns204()
    {
        var recipeId = await CreateRecipeAsync();
        var commentId = await AddCommentAsync(recipeId, "Мой комментарий");

        using var msg = new HttpRequestMessage(HttpMethod.Delete,
            $"/api/v1/recipes/{recipeId}/comments/{commentId}");
        msg.Headers.Authorization = _authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // 8.3 — чужой пользователь → 403
    [Fact]
    public async Task DeleteComment_ByOtherUser_Returns403()
    {
        var recipeId = await CreateRecipeAsync();
        var commentId = await AddCommentAsync(recipeId, "Чужой комментарий");

        // Другой пользователь
        var otherAuth = await fixture.GetAuthHeaderAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Delete,
            $"/api/v1/recipes/{recipeId}/comments/{commentId}");
        msg.Headers.Authorization = otherAuth;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // 8.3 — без JWT → 401
    [Fact]
    public async Task DeleteComment_Unauthorized_Returns401()
    {
        var recipeId = await CreateRecipeAsync();
        var commentId = await AddCommentAsync(recipeId, "Комментарий");

        var response = await _client.DeleteAsync(
            $"/api/v1/recipes/{recipeId}/comments/{commentId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // 8.3 — автор рецепта может удалить чужой комментарий
    [Fact]
    public async Task DeleteComment_ByRecipeAuthor_Returns204()
    {
        var recipeId = await CreateRecipeAsync();

        // Другой пользователь оставляет комментарий
        var otherAuth = await fixture.GetAuthHeaderAsync();

        using var addMsg = new HttpRequestMessage(HttpMethod.Post,
            $"/api/v1/recipes/{recipeId}/comments");
        addMsg.Headers.Authorization = otherAuth;
        addMsg.Content = JsonContent.Create(new { text = "Комментарий от другого" });
        var addResp = await _client.SendAsync(addMsg);
        addResp.EnsureSuccessStatusCode();
        var commentDto = await addResp.Content.ReadFromJsonAsync<CommentDto>();

        // Автор рецепта удаляет
        using var delMsg = new HttpRequestMessage(HttpMethod.Delete,
            $"/api/v1/recipes/{recipeId}/comments/{commentDto!.Id}");
        delMsg.Headers.Authorization = _authHeader;

        var response = await _client.SendAsync(delMsg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Guid> CreateRecipeAsync(string title = "Тестовый рецепт")
    {
        var req = new RecipeRequest(
            Title: title,
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            IsPublic: true,
            Ingredients: [],
            CategoryIds: []);

        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(req);

        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        return dto!.Id;
    }

    private Task<Guid> AddCommentAsync(Guid recipeId, string text)
        => AddCommentAsync(recipeId, text, _authHeader);

    private async Task<Guid> AddCommentAsync(
        Guid recipeId,
        string text,
        System.Net.Http.Headers.AuthenticationHeaderValue auth)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post,
            $"/api/v1/recipes/{recipeId}/comments");
        msg.Headers.Authorization = auth;
        msg.Content = JsonContent.Create(new { text });

        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<CommentDto>();
        return dto!.Id;
    }
}
