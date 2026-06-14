using Recipes.Adapters.Postgresql;
using Recipes.Adapters.Web;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Infrastructure;
using Shared.Hosting.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new InternalControllersFeatureProvider());
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddScoped<IRecipeRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<IIngredientRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<IRecipePhotoRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<ImageSharpThumbnailGenerator>();
builder.Services.AddScoped<IRecipePhotoService, RecipePhotoService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

await app.MigrateDatabaseAsync<Program, RecipeRepository>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecipeRepository>();
    await CookbookSeeder.SeedAsync(db);
}

app.MapHealthChecks("/api/v1/health");
app.MapControllers();

await app.RunAsync();

public partial class Program { }
