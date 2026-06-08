using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ApiGateway.Tests;

public sealed class RoutingTests : IDisposable
{
    private readonly WireMockServer _upstream;
    private readonly WebApplicationFactory<Program> _factory;

    public RoutingTests()
    {
        _upstream = WireMockServer.Start();

        _upstream
            .Given(Request.Create().WithPath("/api/recipes/v1").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[{\"id\":\"test\",\"title\":\"T\",\"description\":\"D\"}]"));

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ReverseProxy:Clusters:recipes-cluster:Destinations:recipes:Address"] =
                            _upstream.Urls[0]
                    });
                });
            });
    }

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health/v1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CookbookRoute_StripsBoundedContextPrefix_AndProxiesToUpstream()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/cookbook/recipes/v1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Проверяем что WireMock получил запрос с трансформированным путём /api/recipes/v1
        var logEntries = _upstream.LogEntries.ToList();
        Assert.Single(logEntries);
        var requestMessage = logEntries[0].RequestMessage;
        Assert.NotNull(requestMessage);
        var path = requestMessage.Path;
        Assert.NotNull(path);
        Assert.Equal("/api/recipes/v1", path);
    }

    public void Dispose()
    {
        _factory.Dispose();
        _upstream.Dispose();
    }
}
