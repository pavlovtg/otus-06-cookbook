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
}
