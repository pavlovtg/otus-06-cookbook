using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipePrivacyTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── GET /api/v1/recipes — фильтрация приватных ────────────────────────────

    [Fact]
    public async Task GetRecipes_AnonymousUser_DoesNotSeePrivateRecipes()
    {
        // Создаём публичный и приватный рецепты от авторизованного пользователя
        await CreateRecipeAsync("Публичный рецепт", isPublic: true);
        await CreateRecipeAsync("Приватный рецепт", isPublic: false);

        // Запрашиваем без токена
        var response = await _client.GetAsync("/api/v1/recipes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.All(result.Items, r => Assert.True(r.IsPublic));
    }

    [Fact]
    public async Task GetRecipes_AuthorUser_SeesOwnPrivateRecipes()
    {
        await CreateRecipeAsync("Публичный рецепт", isPublic: true);
        await CreateRecipeAsync("Приватный рецепт", isPublic: false);

        // Запрашиваем с токеном автора
        using var msg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes");
        msg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
    }

    // ── GET /api/v1/recipes/{id} — 403 для чужого приватного ─────────────────

    [Fact]
    public async Task GetRecipeById_PrivateRecipe_AnonymousUser_Returns403()
    {
        var recipeId = await CreateRecipeAsync("Приватный рецепт", isPublic: false);

        var response = await _client.GetAsync($"/api/v1/recipes/{recipeId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_PrivateRecipe_AuthorUser_Returns200()
    {
        var recipeId = await CreateRecipeAsync("Приватный рецепт", isPublic: false);

        using var msg = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/recipes/{recipeId}");
        msg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_PublicRecipe_AnonymousUser_Returns200()
    {
        var recipeId = await CreateRecipeAsync("Публичный рецепт", isPublic: true);

        var response = await _client.GetAsync($"/api/v1/recipes/{recipeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Guid> CreateRecipeAsync(string title, bool isPublic)
    {
        var req = new RecipeRequest(
            Title: title,
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            IsPublic: isPublic,
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
}
