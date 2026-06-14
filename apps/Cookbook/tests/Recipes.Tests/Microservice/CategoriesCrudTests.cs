using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Recipes.Adapters.Web.Dto;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Microservice;

public sealed class CategoriesCrudTests : IAsyncLifetime
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

    private static CategoryRequest ValidRequest() => new(
        Name: "Первое блюдо",
        Description: "Супы и похлёбки.",
        Type: CategoryTypeDto.MealRole
    );

    // ── GET /api/v1/categories ───────────────────────────────────────────────

    [Fact]
    public async Task GetCategories_Returns200_WithList()
    {
        var response = await _client!.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetCategories_AfterCreate_ContainsCreatedCategory()
    {
        var created = await CreateTestCategoryAsync();

        var response = await _client!.GetAsync("/api/v1/categories");
        var list = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

        Assert.NotNull(list);
        Assert.Contains(list, c => c.Id == created.Id);
    }

    // ── POST /api/v1/categories ──────────────────────────────────────────────

    [Fact]
    public async Task CreateCategory_Returns201_WithCategoryDto()
    {
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", ValidRequest());

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
        var request = ValidRequest() with { Name = "" };
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns400_WhenNameTooLong()
    {
        var request = ValidRequest() with { Name = new string('А', 201) };
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Returns400_WhenDescriptionTooLong()
    {
        var request = ValidRequest() with { Description = new string('А', 2001) };
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_TypeIsSerializedAsSnakeCase()
    {
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", ValidRequest());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"meal_role\"", json);
    }

    // ── PUT /api/v1/categories/{id} ──────────────────────────────────────────

    [Fact]
    public async Task UpdateCategory_Returns204_WhenValid()
    {
        var created = await CreateTestCategoryAsync();

        var updateRequest = ValidRequest() with { Name = "Второе блюдо", Description = "Горячие блюда." };
        var response = await _client!.PutAsJsonAsync($"/api/v1/categories/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client!.GetAsync("/api/v1/categories");
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

        var updateRequest = ValidRequest() with { Name = "" };
        var response = await _client!.PutAsJsonAsync($"/api/v1/categories/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_Returns400_WhenNotFound()
    {
        var response = await _client!.PutAsJsonAsync($"/api/v1/categories/{Guid.NewGuid()}", ValidRequest());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── DELETE /api/v1/categories/{id} ───────────────────────────────────────

    [Fact]
    public async Task DeleteCategory_Returns204_WhenExists()
    {
        var created = await CreateTestCategoryAsync();

        var response = await _client!.DeleteAsync($"/api/v1/categories/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client!.GetAsync("/api/v1/categories");
        var list = await getResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(list);
        Assert.DoesNotContain(list, c => c.Id == created.Id);
    }

    [Fact]
    public async Task DeleteCategory_Returns400_WhenNotFound()
    {
        var response = await _client!.DeleteAsync($"/api/v1/categories/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<CategoryDto> CreateTestCategoryAsync()
    {
        var response = await _client!.PostAsJsonAsync("/api/v1/categories", ValidRequest());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }
}
