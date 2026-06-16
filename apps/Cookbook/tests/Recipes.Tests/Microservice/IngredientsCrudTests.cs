using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class IngredientsCrudTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static IngredientRequest ValidRequest() => new(
        Title: "Тестовый ингредиент",
        Unit: "г",
        DefaultAmount: 100f,
        Category: "vegetables"
    );

    [Fact]
    public async Task CreateIngredient_Returns201_WithIngredientDto()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", ValidRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Тестовый ингредиент", dto.Title);
        Assert.Equal("г", dto.Unit);
        Assert.Equal(100f, dto.DefaultAmount);
        Assert.Equal(IngredientCategoryDto.Vegetables, dto.Category);
        Assert.False(dto.IsSystem);
    }

    [Fact]
    public async Task CreateIngredient_Returns400_WhenTitleTooShort()
    {
        var request = ValidRequest() with { Title = "А" };
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_Returns400_WhenInvalidCategory()
    {
        var request = ValidRequest() with { Category = "unknown_category" };
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_Returns400_WhenDefaultAmountOutOfRange()
    {
        var request = ValidRequest() with { DefaultAmount = 0f };
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredientById_Returns200_WithIngredientDto()
    {
        var created = await CreateTestIngredientAsync();

        var response = await _client.GetAsync($"/api/v1/ingredients/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(dto);
        Assert.Equal(created.Id, dto.Id);
        Assert.Equal(created.Title, dto.Title);
    }

    [Fact]
    public async Task GetIngredientById_Returns400_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/ingredients/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredients_Returns200_WithPagedResult()
    {
        await CreateTestIngredientAsync();

        var response = await _client.GetAsync("/api/v1/ingredients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<IngredientDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.True(result.Total >= 1);
        Assert.Equal(1, result.Page);
        Assert.Equal(100, result.PageSize);
    }

    [Fact]
    public async Task GetIngredients_Returns200_WithExplicitPageAndPageSize()
    {
        var response = await _client.GetAsync("/api/v1/ingredients?page=2&pageSize=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<IngredientDto>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task GetIngredients_Returns200_WhenPageSizeExceedsMax_ClampsTo1000()
    {
        var response = await _client.GetAsync("/api/v1/ingredients?pageSize=5000");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<IngredientDto>>();
        Assert.NotNull(result);
        Assert.Equal(1000, result.PageSize);
    }

    [Fact]
    public async Task GetIngredients_Returns400_WhenPageIsZero()
    {
        var response = await _client.GetAsync("/api/v1/ingredients?page=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredients_Returns400_WhenPageSizeIsZero()
    {
        var response = await _client.GetAsync("/api/v1/ingredients?pageSize=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredients_Returns400_WhenTitleExceeds200Characters()
    {
        var longTitle = new string('А', 201);
        var response = await _client.GetAsync($"/api/v1/ingredients?title={Uri.EscapeDataString(longTitle)}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredients_Returns400_WhenInvalidCategory()
    {
        var response = await _client.GetAsync("/api/v1/ingredients?category=invalid_category");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIngredient_Returns204_WhenValid()
    {
        var created = await CreateTestIngredientAsync();

        var updateRequest = ValidRequest() with { Title = "Обновлённый ингредиент", Unit = "шт." };
        var response = await _client.PutAsJsonAsync($"/api/v1/ingredients/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/ingredients/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(updated);
        Assert.Equal("Обновлённый ингредиент", updated.Title);
        Assert.Equal("шт.", updated.Unit);
    }

    [Fact]
    public async Task UpdateIngredient_Returns400_WhenTitleTooShort()
    {
        var created = await CreateTestIngredientAsync();

        var updateRequest = ValidRequest() with { Title = "А" };
        var response = await _client.PutAsJsonAsync($"/api/v1/ingredients/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteIngredient_Returns204_WhenExists()
    {
        var created = await CreateTestIngredientAsync();

        var response = await _client.DeleteAsync($"/api/v1/ingredients/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/ingredients/{created.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteIngredient_Returns400_WhenNotFound()
    {
        var response = await _client.DeleteAsync($"/api/v1/ingredients/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_CategoryIsSerializedAsSnakeCase()
    {
        var request = ValidRequest() with { Category = "nuts_and_seeds" };

        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"nuts_and_seeds\"", json);
    }

    // ── Delete blocked by recipe usage (8.8) ─────────────────────────────────

    [Fact]
    public async Task DeleteIngredient_Returns400_WhenUsedInRecipe()
    {
        var ingredient = await CreateTestIngredientAsync();

        var recipeRequest = new RecipeRequest(
            Title: "Рецепт с ингредиентом",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [new RecipeIngredientRequest(ingredient.Id, 100m)],
            CategoryIds: []
        );
        using var recipeMsg1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        recipeMsg1.Headers.Authorization = _authHeader;
        recipeMsg1.Content = JsonContent.Create(recipeRequest);
        var recipeResponse = await _client.SendAsync(recipeMsg1);
        recipeResponse.EnsureSuccessStatusCode();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/ingredients/{ingredient.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteIngredient_Returns400_WhenUsedInRecipe_ResponseContainsRecipeTitles()
    {
        var ingredient = await CreateTestIngredientAsync();

        var recipeRequest = new RecipeRequest(
            Title: "Борщ с морковью",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [new RecipeIngredientRequest(ingredient.Id, 100m)],
            CategoryIds: []
        );
        using var recipeMsg2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        recipeMsg2.Headers.Authorization = _authHeader;
        recipeMsg2.Content = JsonContent.Create(recipeRequest);
        var recipeResponse = await _client.SendAsync(recipeMsg2);
        recipeResponse.EnsureSuccessStatusCode();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/ingredients/{ingredient.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);

        var body = await deleteResponse.Content.ReadAsStringAsync();
        Assert.Contains("Борщ с морковью", body);
    }

    private async Task<IngredientDto> CreateTestIngredientAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", ValidRequest());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IngredientDto>())!;
    }
}
