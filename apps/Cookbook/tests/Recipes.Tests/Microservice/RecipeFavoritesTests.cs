using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipeFavoritesTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── POST /api/v1/recipes/{id}/favorites ───────────────────────────────────

    [Fact]
    public async Task AddFavorite_Authorized_Returns201()
    {
        var recipeId = await CreateRecipeAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/favorites");
        msg.Headers.Authorization = _authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddFavorite_Unauthorized_Returns401()
    {
        var recipeId = await CreateRecipeAsync();

        var response = await _client.PostAsync($"/api/v1/recipes/{recipeId}/favorites", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddFavorite_NonExistentRecipe_Returns400()
    {
        var nonExistentId = Guid.NewGuid();

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{nonExistentId}/favorites");
        msg.Headers.Authorization = _authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── DELETE /api/v1/recipes/{id}/favorites ─────────────────────────────────

    [Fact]
    public async Task RemoveFavorite_Authorized_Returns204()
    {
        var recipeId = await CreateRecipeAsync();

        // Сначала добавляем
        using var addMsg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/favorites");
        addMsg.Headers.Authorization = _authHeader;
        (await _client.SendAsync(addMsg)).EnsureSuccessStatusCode();

        // Затем удаляем
        using var removeMsg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/recipes/{recipeId}/favorites");
        removeMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(removeMsg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFavorite_Unauthorized_Returns401()
    {
        var recipeId = await CreateRecipeAsync();

        var response = await _client.DeleteAsync($"/api/v1/recipes/{recipeId}/favorites");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFavorite_WhenNotInFavorites_Returns204()
    {
        var recipeId = await CreateRecipeAsync();

        // Удаляем без предварительного добавления — идемпотентно
        using var msg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/recipes/{recipeId}/favorites");
        msg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // ── GET /api/v1/recipes?favorites=true ────────────────────────────────────

    [Fact]
    public async Task GetRecipes_WithFavoritesTrue_Authorized_ReturnsOnlyFavorites()
    {
        var favoriteRecipeId = await CreateRecipeAsync("Избранный рецепт");
        await CreateRecipeAsync("Обычный рецепт");

        // Добавляем один рецепт в избранное
        using var addMsg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{favoriteRecipeId}/favorites");
        addMsg.Headers.Authorization = _authHeader;
        (await _client.SendAsync(addMsg)).EnsureSuccessStatusCode();

        // Запрашиваем только избранные
        using var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes?favorites=true");
        getMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(getMsg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(favoriteRecipeId, result.Items[0].Id);
    }

    [Fact]
    public async Task GetRecipes_WithFavoritesTrue_Authorized_IsFavoriteIsTrue()
    {
        var recipeId = await CreateRecipeAsync("Рецепт для isFavorite");

        using var addMsg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/favorites");
        addMsg.Headers.Authorization = _authHeader;
        (await _client.SendAsync(addMsg)).EnsureSuccessStatusCode();

        using var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes?favorites=true");
        getMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(getMsg);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.True(result.Items[0].IsFavorite);
    }

    [Fact]
    public async Task GetRecipes_Authorized_IsFavoriteReflectsActualState()
    {
        var favRecipeId = await CreateRecipeAsync("Избранный");
        var notFavRecipeId = await CreateRecipeAsync("Не избранный");

        using var addMsg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{favRecipeId}/favorites");
        addMsg.Headers.Authorization = _authHeader;
        (await _client.SendAsync(addMsg)).EnsureSuccessStatusCode();

        using var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes?sort=title_asc");
        getMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(getMsg);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);

        var favItem = result.Items.FirstOrDefault(r => r.Id == favRecipeId);
        var notFavItem = result.Items.FirstOrDefault(r => r.Id == notFavRecipeId);

        Assert.NotNull(favItem);
        Assert.NotNull(notFavItem);
        Assert.True(favItem.IsFavorite);
        Assert.False(notFavItem.IsFavorite);
    }

    [Fact]
    public async Task GetRecipes_WithFavoritesTrue_Unauthorized_ReturnsAllPublic()
    {
        // Без авторизации favorites=true игнорируется (нет userId), возвращаются все публичные
        await CreateRecipeAsync("Рецепт 1");
        await CreateRecipeAsync("Рецепт 2");

        var response = await _client.GetAsync("/api/v1/recipes?favorites=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        // Без userId фильтр не применяется — возвращаются все публичные рецепты
        Assert.True(result.Total >= 2);
        // isFavorite = null для неавторизованных
        Assert.All(result.Items, r => Assert.Null(r.IsFavorite));
    }

    [Fact]
    public async Task GetRecipes_WithFavoritesTrue_WhenNoFavorites_ReturnsEmpty()
    {
        await CreateRecipeAsync("Рецепт без избранного");

        using var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes?favorites=true");
        getMsg.Headers.Authorization = _authHeader;
        var response = await _client.SendAsync(getMsg);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
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
}
