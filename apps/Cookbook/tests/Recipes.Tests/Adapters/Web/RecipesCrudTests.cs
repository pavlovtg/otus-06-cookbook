using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Recipes.Adapters.Web.Dto;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Web;

public sealed class RecipesCrudTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Recipes"] = _postgres.GetConnectionString()
                    });
                });
            });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_factory is not null)
            await _factory.DisposeAsync();

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateRecipe_Returns201_WithRecipeDto()
    {
        var request = new RecipeRequest(
            Title: "Тестовый рецепт",
            Description: "Описание тестового рецепта",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1. Приготовить."
        );

        var response = await _client!.PostAsJsonAsync("/api/v1/recipes", request);

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
            Instructions: "Инструкции"
        );

        var response = await _client!.PostAsJsonAsync("/api/v1/recipes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_Returns200_WithRecipeDto()
    {
        var created = await CreateTestRecipeAsync();

        var response = await _client!.GetAsync($"/api/v1/recipes/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(dto);
        Assert.Equal(created.Id, dto.Id);
        Assert.Equal(created.Title, dto.Title);
    }

    [Fact]
    public async Task GetRecipeById_Returns400_WhenNotFound()
    {
        var response = await _client!.GetAsync($"/api/v1/recipes/{Guid.NewGuid()}");

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
            Instructions: "Новые инструкции"
        );

        var response = await _client!.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client!.GetAsync($"/api/v1/recipes/{created.Id}");
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
            Instructions: "Инструкции"
        );

        var response = await _client!.PutAsJsonAsync($"/api/v1/recipes/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRecipe_Returns204_WhenExists()
    {
        var created = await CreateTestRecipeAsync();

        var response = await _client!.DeleteAsync($"/api/v1/recipes/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client!.GetAsync($"/api/v1/recipes/{created.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteRecipe_Returns400_WhenNotFound()
    {
        var response = await _client!.DeleteAsync($"/api/v1/recipes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<RecipeDto> CreateTestRecipeAsync()
    {
        var request = new RecipeRequest(
            Title: "Рецепт для теста",
            Description: "Описание для теста",
            CookingTime: 45,
            Difficulty: "everyday",
            Servings: 3,
            Instructions: "Шаг 1. Тест."
        );

        var response = await _client!.PostAsJsonAsync("/api/v1/recipes", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>();
        return dto!;
    }
}
