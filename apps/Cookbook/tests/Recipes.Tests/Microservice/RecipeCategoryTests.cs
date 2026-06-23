using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipeCategoryTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

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
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [category.Id]);

        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(request);
        var response = await _client.SendAsync(msg);

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
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [cat1.Id]);

        using var createMsg1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        createMsg1.Headers.Authorization = _authHeader;
        createMsg1.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createMsg1);
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var updateRequest = new RecipeRequest(
            Title: "Рецепт",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [cat2.Id]);

        using var updateMsg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/recipes/{created.Id}");
        updateMsg.Headers.Authorization = _authHeader;
        updateMsg.Content = JsonContent.Create(updateRequest);
        var updateResponse = await _client.SendAsync(updateMsg);
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
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [category.Id]);

        using var createMsg2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        createMsg2.Headers.Authorization = _authHeader;
        createMsg2.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createMsg2);
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
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [Guid.NewGuid()]);

        using var msg3 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        msg3.Headers.Authorization = _authHeader;
        msg3.Content = JsonContent.Create(request);
        var response = await _client.SendAsync(msg3);

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
            IsPublic: true,
            Ingredients: [],
            CategoryIds: [category.Id]);

        using var createMsg4 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        createMsg4.Headers.Authorization = _authHeader;
        createMsg4.Content = JsonContent.Create(createRequest);
        var createResponse = await _client.SendAsync(createMsg4);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(created);

        var listResponse = await _client.GetAsync("/api/v1/recipes");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<RecipeShortDto>>();
        Assert.NotNull(list);

        var found = list.Items.FirstOrDefault(r => r.Id == created.Id);
        Assert.NotNull(found);
        Assert.Single(found.CategoryIds);
        Assert.Contains(category.Id, found.CategoryIds);
    }

    private async Task<CategoryDto> CreateTestCategoryAsync(
        string name = "Тестовая категория",
        CategoryTypeDto type = CategoryTypeDto.MealRole)
    {
        var adminHeader = await fixture.GetAdminAuthHeaderAsync();
        var request = new CategoryRequest(Name: name, Description: "Описание", Type: type);
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = adminHeader;
        msg.Content = JsonContent.Create(request);
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }
}
