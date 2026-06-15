using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Recipes.Adapters.Web.Dto;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Microservice;

public sealed class RecipeCategoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    private RecipeMicroserviceHost? _host;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _host = new RecipeMicroserviceHost(_postgres.GetConnectionString()).EnsureServer();
        _client = _host.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_host is not null)
            await _host.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateRecipe_WithCategoryIds_Returns201_WithCategoryIds()
    {
        var category = await CreateTestCategoryAsync();

        var request = new RecipeRequest(
            Title: "Рецепт с категорией",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: [category.Id]);

        var response = await _client!.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.CategoryIds);
        Assert.Contains(category.Id, dto.CategoryIds);
    }

    [Fact]
    public async Task UpdateRecipe_WithCategoryIds_UpdatesCategoryIds()
    {
        var cat1 = await CreateTestCategoryAsync("Завтрак", CategoryTypeDto.MealRole);
        var cat2 = await CreateTestCategoryAsync("Итальянская", CategoryTypeDto.Cuisine);

        var createRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: [cat1.Id]);

        var createResponse = await _client!.PostAsJsonAsync("/api/v1/recipes", createRequest);
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
            CategoryIds: [cat2.Id]);

        var updateResponse = await _client!.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client!.GetAsync($"/api/v1/recipes/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(updated);
        Assert.Single(updated.CategoryIds);
        Assert.Contains(cat2.Id, updated.CategoryIds);
        Assert.DoesNotContain(cat1.Id, updated.CategoryIds);
    }

    [Fact]
    public async Task GetRecipe_Returns_CategoryIds()
    {
        var category = await CreateTestCategoryAsync();

        var createRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: [category.Id]);

        var createResponse = await _client!.PostAsJsonAsync("/api/v1/recipes", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var getResponse = await _client!.GetAsync($"/api/v1/recipes/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var dto = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.CategoryIds);
        Assert.Contains(category.Id, dto.CategoryIds);
    }

    [Fact]
    public async Task CreateRecipe_WithNonExistentCategoryId_Returns400()
    {
        var request = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: [Guid.NewGuid()]);

        var response = await _client!.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<CategoryDto> CreateTestCategoryAsync(
        string name = "Тестовая категория",
        CategoryTypeDto type = CategoryTypeDto.MealRole)
    {
        var request = new CategoryRequest(Name: name, Description: "Описание", Type: type);
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }
}
