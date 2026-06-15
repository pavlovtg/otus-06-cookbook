using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipesCrudTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateRecipe_Returns201_WithRecipeDto()
    {
        var request = new RecipeRequest(
            Title: "Тестовый рецепт",
            Description: "Описание тестового рецепта",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1. Приготовить.",
            Ingredients: [],
            CategoryIds: []
        );

        var response = await _client.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Тестовый рецепт", dto.Title);
        Assert.Equal(30, dto.CookingTime);
        Assert.Equal("easy", dto.Difficulty);
        Assert.Equal(2, dto.Servings);
    }

    [Fact]
    public async Task CreateRecipe_Returns400_WhenTitleEmpty()
    {
        var request = new RecipeRequest(
            Title: "",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Инструкции",
            Ingredients: [],
            CategoryIds: []
        );

        var response = await _client.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_Returns200_WithRecipeDto()
    {
        var created = await CreateTestRecipeAsync();

        var response = await _client.GetAsync($"/api/v1/recipes/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.Equal(created.Id, dto.Id);
        Assert.Equal(created.Title, dto.Title);
    }

    [Fact]
    public async Task GetRecipeById_Returns400_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/recipes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRecipe_Returns204_WhenValid()
    {
        var created = await CreateTestRecipeAsync();

        var updateRequest = new RecipeRequest(
            Title: "Обновлённый рецепт",
            Description: "Новое описание",
            CookingTime: 60,
            Difficulty: "festive",
            Servings: 4,
            Instructions: "Новые инструкции",
            Ingredients: [],
            CategoryIds: []
        );

        var response = await _client.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(updated);
        Assert.Equal("Обновлённый рецепт", updated.Title);
        Assert.Equal(60, updated.CookingTime);
    }

    [Fact]
    public async Task UpdateRecipe_Returns400_WhenTitleEmpty()
    {
        var created = await CreateTestRecipeAsync();

        var updateRequest = new RecipeRequest(
            Title: "",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Инструкции",
            Ingredients: [],
            CategoryIds: []
        );

        var response = await _client.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRecipe_Returns204_WhenExists()
    {
        var created = await CreateTestRecipeAsync();

        var response = await _client.DeleteAsync($"/api/v1/recipes/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteRecipe_Returns400_WhenNotFound()
    {
        var response = await _client.DeleteAsync($"/api/v1/recipes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Ingredients (8.6) ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRecipe_WithIngredients_GetById_ReturnsIngredients()
    {
        var ingredient = await CreateTestIngredientAsync();

        var request = new RecipeRequest(
            Title: "Рецепт с ингредиентами",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [new RecipeIngredientRequest(ingredient.Id, 150m)],
            CategoryIds: []
        );

        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var dto = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.Ingredients);
        Assert.Equal(ingredient.Id, dto.Ingredients[0].IngredientId);
        Assert.Equal(150m, dto.Ingredients[0].Amount);
    }

    // ── Update ingredients (8.7) ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateRecipe_WithNewIngredients_ReplacesIngredients()
    {
        var ingredient1 = await CreateTestIngredientAsync();
        var ingredient2 = await CreateTestIngredientAsync("Лук", "шт.");

        var createRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [new RecipeIngredientRequest(ingredient1.Id, 100m)],
            CategoryIds: []
        );

        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var updateRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [
                new RecipeIngredientRequest(ingredient1.Id, 200m),
                new RecipeIngredientRequest(ingredient2.Id, 3m),
            ],
            CategoryIds: []
        );

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(updated);
        Assert.Equal(2, updated.Ingredients.Count);
    }

    [Fact]
    public async Task UpdateRecipe_WithEmptyIngredients_ClearsIngredients()
    {
        var ingredient = await CreateTestIngredientAsync();

        var createRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [new RecipeIngredientRequest(ingredient.Id, 100m)],
            CategoryIds: []
        );

        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var updateRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: []
        );

        await _client.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(updated);
        Assert.Empty(updated.Ingredients);
    }

    private async Task<RecipeDto> CreateTestRecipeAsync()
    {
        var request = new RecipeRequest(
            Title: "Рецепт для теста",
            Description: "Описание для теста",
            CookingTime: 45,
            Difficulty: "everyday",
            Servings: 3,
            Instructions: "Шаг 1. Тест.",
            Ingredients: [],
            CategoryIds: []
        );

        var response = await _client.PostAsJsonAsync("/api/v1/recipes", request);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<RecipeDto>())!;
    }

    private async Task<IngredientDto> CreateTestIngredientAsync(string title = "Морковь", string unit = "г")
    {
        var request = new IngredientRequest(
            Title: title,
            Unit: unit,
            DefaultAmount: 100f,
            Category: "vegetables"
        );

        var response = await _client.PostAsJsonAsync("/api/v1/ingredients", request);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<IngredientDto>())!;
    }
}
