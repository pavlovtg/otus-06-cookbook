using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class GetRecipesTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var response = await _client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_AfterCreate_Returns200_WithPagedResult()
    {
        var createRequest = new RecipeRequest(
            Title: "Рецепт для списка",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: []
        );
        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync("/api/v1/recipes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.True(result.Total >= 1);
        Assert.Equal(1, result.Page);
        Assert.Equal(18, result.PageSize);
    }

    [Fact]
    public async Task GetRecipes_WithPageParams_ReturnsPaginatedResult()
    {
        for (var i = 1; i <= 3; i++)
        {
            var req = new RecipeRequest(
                Title: $"Рецепт {i}",
                Description: "Описание",
                CookingTime: 30,
                Difficulty: "easy",
                Servings: 2,
                Instructions: "Шаг 1.",
                Ingredients: [],
                CategoryIds: []
            );
            (await _client.PostAsJsonAsync("/api/v1/recipes", req)).EnsureSuccessStatusCode();
        }

        var response = await _client.GetAsync("/api/v1/recipes?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.True(result.Total >= 3);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetRecipes_InvalidPage_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/recipes?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_InvalidPageSize_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/recipes?pageSize=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_QueryTooLong_Returns400()
    {
        var response = await _client.GetAsync($"/api/v1/recipes?q={new string('а', 301)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_QueryExactlyMaxLength_Returns200()
    {
        var response = await _client.GetAsync($"/api/v1/recipes?q={new string('а', 300)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Search (q) ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipes_WithQuery_ReturnsMatchingRecipes()
    {
        await CreateRecipeAsync("Борщ украинский", "Классический рецепт");
        await CreateRecipeAsync("Пельмени сибирские", "Традиционное блюдо");

        var response = await _client.GetAsync("/api/v1/recipes?q=борщ");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.All(result.Items, r => Assert.Contains("борщ", r.Title.ToLower()));
    }

    [Fact]
    public async Task GetRecipes_WithQuery_NoMatch_ReturnsEmptyList()
    {
        await CreateRecipeAsync("Борщ", "Описание");

        var response = await _client.GetAsync("/api/v1/recipes?q=несуществующееслово12345");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetRecipes_WithMultiWordQuery_AppliesAndLogic()
    {
        await CreateRecipeAsync("Борщ украинский", "Классический рецепт");
        await CreateRecipeAsync("Борщ постный", "Без мяса");
        await CreateRecipeAsync("Суп украинский", "Другой суп");

        var response = await _client.GetAsync("/api/v1/recipes?q=борщ+украинский");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Борщ украинский", result.Items[0].Title);
    }

    // ── Sort ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipes_SortTitleAsc_ReturnsSortedAscending()
    {
        await CreateRecipeAsync("Щи", "Описание");
        await CreateRecipeAsync("Борщ", "Описание");
        await CreateRecipeAsync("Окрошка", "Описание");

        var response = await _client.GetAsync("/api/v1/recipes?sort=title_asc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        var titles = result.Items.Select(r => r.Title).ToList();
        Assert.Equal(titles.OrderBy(t => t).ToList(), titles);
    }

    [Fact]
    public async Task GetRecipes_SortTitleDesc_ReturnsSortedDescending()
    {
        await CreateRecipeAsync("Щи", "Описание");
        await CreateRecipeAsync("Борщ", "Описание");
        await CreateRecipeAsync("Окрошка", "Описание");

        var response = await _client.GetAsync("/api/v1/recipes?sort=title_desc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        var titles = result.Items.Select(r => r.Title).ToList();
        Assert.Equal(titles.OrderByDescending(t => t).ToList(), titles);
    }

    [Fact]
    public async Task GetRecipes_UnknownSort_DefaultsToTitleAsc()
    {
        await CreateRecipeAsync("Щи", "Описание");
        await CreateRecipeAsync("Борщ", "Описание");

        var response = await _client.GetAsync("/api/v1/recipes?sort=unknown_value");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        var titles = result.Items.Select(r => r.Title).ToList();
        Assert.Equal(titles.OrderBy(t => t).ToList(), titles);
    }

    // ── Search by ingredients ─────────────────────────────────────────────────

    [Fact]
    public async Task GetRecipes_WithQuery_TwoIngredients_ReturnsOnlyRecipeContainingBoth()
    {
        var carrotId = await CreateIngredientAsync("Морковь");
        var potatoId = await CreateIngredientAsync("Картофель");

        await CreateRecipeWithIngredientsAsync("Суп с морковью и картофелем", [carrotId, potatoId]);
        await CreateRecipeWithIngredientsAsync("Морковный салат", [carrotId]);
        await CreateRecipeWithIngredientsAsync("Картофельное пюре", [potatoId]);

        var response = await _client.GetAsync("/api/v1/recipes?q=морковь+картофель");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Суп с морковью и картофелем", result.Items[0].Title);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task CreateRecipeAsync(string title, string description)
    {
        var req = new RecipeRequest(
            Title: title,
            Description: description,
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: []);
        (await _client.PostAsJsonAsync("/api/v1/recipes", req)).EnsureSuccessStatusCode();
    }

    private async Task<Guid> CreateIngredientAsync(string title)
    {
        var req = new IngredientRequest(
            Title: title,
            Unit: "г",
            DefaultAmount: 100f,
            Category: "vegetables");
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", req);
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<IngredientDto>();
        return dto!.Id;
    }

    private async Task CreateRecipeWithIngredientsAsync(string title, IEnumerable<Guid> ingredientIds)
    {
        var ingredients = ingredientIds
            .Select(id => new RecipeIngredientRequest(IngredientId: id, Amount: 100m))
            .ToList();
        var req = new RecipeRequest(
            Title: title,
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: ingredients,
            CategoryIds: []);
        (await _client.PostAsJsonAsync("/api/v1/recipes", req)).EnsureSuccessStatusCode();
    }
}
