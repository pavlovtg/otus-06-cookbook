using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Database.ConnectionStrings;

namespace Recipes.Tests.Microservice;

internal sealed class RecipeMicroserviceHost : WebApplicationFactory<Program>
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan _defaultPeriod = TimeSpan.FromMilliseconds(300);

    private readonly string _databaseConnectionString;

    public RecipeMicroserviceHost(string databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString;
    }

    public RecipeMicroserviceHost EnsureServer()
    {
        // This forces WebApplicationFactory to bootstrap the server
        using var client = CreateDefaultClient();

        if (!WaitForReady(client, _defaultTimeout))
        {
            throw new InvalidOperationException("Host isn't ready.");
        }

        return this;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder.ConfigureHostConfiguration(
            x =>
            {
                var databaseConfiguration = new { DatabaseConnection = _databaseConnectionString.FromConnectionString() };
                AddConfigurationToBuilder(x, databaseConfiguration);
                var suppressLogging = new
				{
					Logging = new Dictionary<string, string>
					{
						{ "LogLevel:Microsoft.EntityFrameworkCore", "Warning" },
						{ "LogLevel:Microsoft.AspNetCore", "Warning" },
					}
				};
				AddConfigurationToBuilder(x, suppressLogging);
            });

        return base.CreateHost(builder);
    }

    private static bool WaitForReady(HttpClient client, TimeSpan timeout)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException("Timeout less or equal zero.");

        return SpinWait.SpinUntil(
            () =>
            {
                Thread.Sleep(_defaultPeriod);

                var response = client.GetAsync("/api/v1/health").GetAwaiter().GetResult();

                return response.StatusCode == HttpStatusCode.OK;
            },
            timeout);
    }

    private void AddConfigurationToBuilder<TValue>(IConfigurationBuilder builder, TValue config)
    {
        if (config == null)
		{
			return;
		}
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, config);
        stream.Seek(0, SeekOrigin.Begin);
        builder.AddJsonStream(stream);
    }
}
