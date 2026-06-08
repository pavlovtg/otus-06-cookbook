using Microsoft.EntityFrameworkCore;
using Recipes.Adapters.Postgresql;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new InternalControllersFeatureProvider());
    });
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<RecipesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Recipes")));

builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecipesDbContext>();
    await db.Database.MigrateAsync();
}

app.MapHealthChecks("/api/health/v1");
app.MapControllers();

await app.RunAsync();

public partial class Program { }
