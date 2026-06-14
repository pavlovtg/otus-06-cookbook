using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Recipes.Adapters.Web.Dto;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Microservice;

public sealed class GetRecipesTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    private RecipeMicroserviceHost? _host;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _host = new RecipeMicroserviceHost(_postgres.GetConnectionString()).EnsureServer();
    }

    public async Task DisposeAsync()
    {
        if (_host is not null)
            await _host.DisposeAsync();

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var client = _host!.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_Returns200_WithNonEmptyArray()
    {
        var client = _host!.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var recipes = await response.Content.ReadFromJsonAsync<RecipeDto[]>();
        Assert.NotNull(recipes);
        Assert.NotEmpty(recipes);
    }
}
