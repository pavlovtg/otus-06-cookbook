using ApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapHealthChecks("/api/v1/health");

// OpenAPI документ из контракта recipes.yaml, переписанный под gateway-префикс /api/cookbook
var contractPath = builder.Configuration["OpenApi:RecipesContractPath"]
    ?? "contracts/cookbook/recipes.yaml";
if (!Path.IsPathRooted(contractPath))
{
    contractPath = Path.Combine(AppContext.BaseDirectory, contractPath);
}
var pathPrefix = builder.Configuration["OpenApi:RecipesPathPrefix"] ?? "cookbook";

string? openApiJson = null;
try
{
    if (File.Exists(contractPath))
    {
        openApiJson = OpenApiDocumentLoader.LoadAndRewrite(contractPath, pathPrefix);
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to load OpenAPI contract from {Path}", contractPath);
}

app.MapGet("/api-docs/recipes.json", () =>
    openApiJson is null
        ? Results.NotFound(new { error = $"OpenAPI contract not found at '{contractPath}'" })
        : Results.Content(openApiJson, "application/json"));

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "api-docs";
    c.SwaggerEndpoint("/api-docs/recipes.json", "Recipes API");
    c.DocumentTitle = "Cookbook API";
});

app.MapReverseProxy();

await app.RunAsync();

public partial class Program { }
