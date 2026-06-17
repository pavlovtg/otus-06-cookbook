using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipeRatingTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── PUT /api/v1/recipes/{id}/rating ───────────────────────────────────────

    // 8.4
    [Fact]
    public async Task SetRating_Authorized_Returns200WithCorrectSummary()
    {
        var recipeId = await CreateRecipeAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(new { value = 4 });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content.ReadFromJsonAsync<RatingSummaryDto>();
        Assert.NotNull(summary);
        Assert.NotNull(summary.AverageRating);
        Assert.Equal(4, summary.MyRating);
    }

    // 8.5
    [Fact]
    public async Task SetRating_SecondTime_ReplacesFirstRating()
    {
        var recipeId = await CreateRecipeAsync();

        // Первая оценка
        using var first = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        first.Headers.Authorization = _authHeader;
        first.Content = JsonContent.Create(new { value = 2 });
        (await _client.SendAsync(first)).EnsureSuccessStatusCode();

        // Вторая оценка
        using var second = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        second.Headers.Authorization = _authHeader;
        second.Content = JsonContent.Create(new { value = 5 });
        var response = await _client.SendAsync(second);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content.ReadFromJsonAsync<RatingSummaryDto>();
        Assert.NotNull(summary);
        Assert.Equal(5, summary.MyRating);
        Assert.Equal(5f, summary.AverageRating);
    }

    // 8.6
    [Fact]
    public async Task SetRating_Unauthorized_Returns401()
    {
        var recipeId = await CreateRecipeAsync();

        var response = await _client.PutAsJsonAsync($"/api/v1/recipes/{recipeId}/rating", new { value = 3 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // 8.7
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task SetRating_OutOfRangeValue_Returns400(int value)
    {
        var recipeId = await CreateRecipeAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(new { value });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── DELETE /api/v1/recipes/{id}/rating ────────────────────────────────────

    // 8.8
    [Fact]
    public async Task DeleteRating_ExistingRating_Returns204()
    {
        var recipeId = await CreateRecipeAsync();

        // Сначала выставляем оценку
        using var setMsg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        setMsg.Headers.Authorization = _authHeader;
        setMsg.Content = JsonContent.Create(new { value = 3 });
        (await _client.SendAsync(setMsg)).EnsureSuccessStatusCode();

        // Удаляем
        using var delMsg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/recipes/{recipeId}/rating");
        delMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(delMsg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // 8.9
    [Fact]
    public async Task DeleteRating_NoRatingExists_Returns400()
    {
        var recipeId = await CreateRecipeAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/recipes/{recipeId}/rating");
        msg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/v1/recipes?sort=rating_desc ──────────────────────────────────

    // 8.10
    [Fact]
    public async Task GetRecipes_SortRatingDesc_ReturnsSortedByAverageRatingDescending()
    {
        var recipe1Id = await CreateRecipeAsync("Рецепт А");
        var recipe2Id = await CreateRecipeAsync("Рецепт Б");
        var recipe3Id = await CreateRecipeAsync("Рецепт В");

        // Второй пользователь для разных оценок
        var auth2 = await fixture.GetAuthHeaderAsync();

        await SetRatingAsync(recipe1Id, 2, _authHeader);
        await SetRatingAsync(recipe2Id, 5, _authHeader);
        await SetRatingAsync(recipe3Id, 3, _authHeader);

        // Добавляем вторую оценку для recipe2, чтобы avg был точным
        await SetRatingAsync(recipe2Id, 5, auth2);

        using var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes?sort=rating_desc");
        getMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(getMsg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);

        // Рецепты с оценками должны идти первыми, в порядке убывания averageRating
        var ratedItems = result.Items
            .Where(r => r.AverageRating.HasValue)
            .ToList();

        Assert.True(ratedItems.Count >= 3);

        var ratings = ratedItems.Select(r => r.AverageRating!.Value).ToList();
        for (var i = 0; i < ratings.Count - 1; i++)
            Assert.True(ratings[i] >= ratings[i + 1],
                $"Ожидался убывающий порядок: [{string.Join(", ", ratings)}]");
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

    private async Task SetRatingAsync(
        Guid recipeId,
        int value,
        System.Net.Http.Headers.AuthenticationHeaderValue auth)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{recipeId}/rating");
        msg.Headers.Authorization = auth;
        msg.Content = JsonContent.Create(new { value });
        (await _client.SendAsync(msg)).EnsureSuccessStatusCode();
    }
}
