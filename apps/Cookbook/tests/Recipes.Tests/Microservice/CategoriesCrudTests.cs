using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class CategoriesCrudTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static CategoryRequest ValidRequest() => new(
        Name: "Первое блюдо",
        Description: "Супы и похлёбки.",
        Type: CategoryTypeDto.MealRole
    );

    // ── GET /api/v1/categories ───────────────────────────────────────────────

    [Fact]
    public async Task GetCategories_Returns200_WithList()
    {
        var response = await _client.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetCategories_AfterCreate_ContainsCreatedCategory()
    {
        var created = await CreateTestCategoryAsync();

        var response = await _client.GetAsync("/api/v1/categories");
        var list = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

        Assert.NotNull(list);
        Assert.Contains(list, c => c.Id == created.Id);
    }

    // ── POST /api/v1/categories ──────────────────────────────────────────────

    [Fact]
    public async Task CreateCategory_Returns401_WhenNotAuthenticated()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categories", ValidRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns403_WhenNotAdmin()
    {
        var authHeader = await fixture.GetAuthHeaderAsync("user");
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        request.Headers.Authorization = authHeader;
        request.Content = JsonContent.Create(ValidRequest());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns201_WithCategoryDto()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        request.Headers.Authorization = authHeader;
        request.Content = JsonContent.Create(ValidRequest());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Первое блюдо", dto.Name);
        Assert.Equal("Супы и похлёбки.", dto.Description);
        Assert.Equal(CategoryTypeDto.MealRole, dto.Type);
    }

    [Fact]
    public async Task CreateCategory_Returns400_WhenNameEmpty()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        var request = ValidRequest() with { Name = "" };
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns400_WhenNameTooLong()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        var request = ValidRequest() with { Name = new string('А', 201) };
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns400_WhenDescriptionTooLong()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        var request = ValidRequest() with { Description = new string('А', 2001) };
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_TypeIsSerializedAsSnakeCase()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(ValidRequest());

        var response = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"meal_role\"", json);
    }

    // ── PUT /api/v1/categories/{id} ──────────────────────────────────────────

    [Fact]
    public async Task UpdateCategory_Returns401_WhenNotAuthenticated()
    {
        var created = await CreateTestCategoryAsync();

        var response = await _client.PutAsJsonAsync($"/api/v1/categories/{created.Id}", ValidRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_Returns403_WhenNotAdmin()
    {
        var created = await CreateTestCategoryAsync();
        var authHeader = await fixture.GetAuthHeaderAsync("user");
        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categories/{created.Id}");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(ValidRequest() with { Name = "Другое" });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_Returns204_WhenValid()
    {
        var created = await CreateTestCategoryAsync();
        var authHeader = await fixture.GetAdminAuthHeaderAsync();

        var updateRequest = ValidRequest() with { Name = "Второе блюдо", Description = "Горячие блюда." };
        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categories/{created.Id}");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(updateRequest);

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/api/v1/categories");
        var list = await getResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(list);
        var updated = list.FirstOrDefault(c => c.Id == created.Id);
        Assert.NotNull(updated);
        Assert.Equal("Второе блюдо", updated.Name);
        Assert.Equal("Горячие блюда.", updated.Description);
    }

    [Fact]
    public async Task UpdateCategory_Returns400_WhenNameEmpty()
    {
        var created = await CreateTestCategoryAsync();
        var authHeader = await fixture.GetAdminAuthHeaderAsync();

        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categories/{created.Id}");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(ValidRequest() with { Name = "" });

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_Returns400_WhenNotFound()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var msg = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categories/{Guid.NewGuid()}");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(ValidRequest());

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── DELETE /api/v1/categories/{id} ───────────────────────────────────────

    [Fact]
    public async Task DeleteCategory_Returns401_WhenNotAuthenticated()
    {
        var created = await CreateTestCategoryAsync();

        var response = await _client.DeleteAsync($"/api/v1/categories/{created.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_Returns403_WhenNotAdmin()
    {
        var created = await CreateTestCategoryAsync();
        var authHeader = await fixture.GetAuthHeaderAsync("user");
        using var msg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/categories/{created.Id}");
        msg.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_Returns204_WhenExists()
    {
        var created = await CreateTestCategoryAsync();
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var msg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/categories/{created.Id}");
        msg.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/api/v1/categories");
        var list = await getResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(list);
        Assert.DoesNotContain(list, c => c.Id == created.Id);
    }

    [Fact]
    public async Task DeleteCategory_Returns400_WhenNotFound()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var msg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/categories/{Guid.NewGuid()}");
        msg.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<CategoryDto> CreateTestCategoryAsync()
    {
        var authHeader = await fixture.GetAdminAuthHeaderAsync();
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories");
        msg.Headers.Authorization = authHeader;
        msg.Content = JsonContent.Create(ValidRequest());
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }
}
