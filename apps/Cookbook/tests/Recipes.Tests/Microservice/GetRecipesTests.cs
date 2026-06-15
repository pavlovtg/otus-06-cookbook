using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
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
    public async Task GetRecipes_AfterCreate_Returns200_WithNonEmptyArray()
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

        var recipes = await response.Content.ReadFromJsonAsync<RecipeDto[]>();
        Assert.NotNull(recipes);
        Assert.NotEmpty(recipes);
    }
}
