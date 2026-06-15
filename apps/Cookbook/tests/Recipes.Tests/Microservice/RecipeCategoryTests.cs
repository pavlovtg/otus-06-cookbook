using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipeCategoryTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

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

        var response = await _client.PostAsJsonAsync("/api/v1/recipes", request);

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
            CategoryIds: [cat2.Id]);

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
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

        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/v1/recipes/{created.Id}");
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

        var response = await _client.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipesList_AfterCreateWithCategory_ContainsCategoryId()
    {
        var category = await CreateTestCategoryAsync();

        var createRequest = new RecipeRequest(
            Title: "Рецепт в списке",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: [category.Id]);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/recipes", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var listResponse = await _client.GetAsync("/api/v1/recipes");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var list = await listResponse.Content.ReadFromJsonAsync<List<RecipeShortDto>>();
        Assert.NotNull(list);

        var found = list.FirstOrDefault(r => r.Id == created.Id);
        Assert.NotNull(found);
        Assert.Single(found.CategoryIds);
        Assert.Contains(category.Id, found.CategoryIds);
    }

    private async Task<CategoryDto> CreateTestCategoryAsync(
        string name = "Тестовая категория",
        CategoryTypeDto type = CategoryTypeDto.MealRole)
    {
        var request = new CategoryRequest(Name: name, Description: "Описание", Type: type);
        var response = await _client.PostAsJsonAsync("/api/v1/categories", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }
}
