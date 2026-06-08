using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Recipes.Adapters.Web.Dto;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Web;

public sealed class GetRecipesTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    private WebApplicationFactory<Program>? _factory;

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
    }

    public async Task DisposeAsync()
    {
        if (_factory is not null)
            await _factory.DisposeAsync();

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var client = _factory!.CreateClient();

        var response = await client.GetAsync("/api/health/v1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_Returns200_WithNonEmptyArray()
    {
        var client = _factory!.CreateClient();

        var response = await client.GetAsync("/api/recipes/v1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var recipes = await response.Content.ReadFromJsonAsync<RecipeDto[]>();
        Assert.NotNull(recipes);
        Assert.NotEmpty(recipes);
    }
}
