using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddScoped<ICategoryRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IRecipeCommentService, RecipeCommentService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ImageSharpThumbnailGenerator>();
builder.Services.AddScoped<IRecipePhotoService, RecipePhotoService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMealPlanRepository>(sp => sp.GetRequiredService<RecipeRepository>());
builder.Services.AddScoped<MealPlanService>();

var jwtSecret = builder.Configuration["JWT:Secret"]
    ?? throw new InvalidOperationException("JWT:Secret is not configured.");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "cookbook";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "cookbook";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

await app.MigrateDatabaseAsync<Program, RecipeRepository>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecipeRepository>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var photosPath = app.Configuration["Seeder:PhotosPath"];
    await CookbookSeeder.SeedAsync(db, passwordHasher, photosPath);
}

app.MapHealthChecks("/api/v1/health");
app.MapControllers();

await app.RunAsync();

public partial class Program { }
