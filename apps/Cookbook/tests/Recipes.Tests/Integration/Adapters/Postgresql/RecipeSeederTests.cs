using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class RecipeSeederTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    private RepositoryFactory<RecipeRepository> _factory = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new RepositoryFactory<RecipeRepository>(
            _postgres.GetConnectionString(),
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));

        await _factory.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task SeedAsync_EmptyDb_InsertsAllSeedRecipes()
    {
        await using var seedCtx = _factory.Create();
        await RecipeSeeder.SeedAsync(seedCtx);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Recipes.CountAsync();

        Assert.Equal(SeedData.Recipes.Length, count);
    }

    [Fact]
    public async Task SeedAsync_SeedData_MatchesExpected()
    {
        await using var seedCtx = _factory.Create();
        await RecipeSeeder.SeedAsync(seedCtx);

        var expected = SeedData.Recipes[0];

        await using var readCtx = _factory.Create();
        var actual = await readCtx.Recipes.FindAsync([expected.Id]);

        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.CookingTime, actual.CookingTime);
        Assert.Equal(expected.Difficulty, actual.Difficulty);
        Assert.Equal(expected.Servings, actual.Servings);
        Assert.Equal(expected.Instructions, actual.Instructions);
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicate()
    {
        await using (var ctx1 = _factory.Create())
            await RecipeSeeder.SeedAsync(ctx1);

        await using (var ctx2 = _factory.Create())
            await RecipeSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Recipes.CountAsync();

        Assert.Equal(SeedData.Recipes.Length, count);
    }

    [Fact]
    public async Task SeedAsync_ExistingRecord_UpdatesIt()
    {
        // Первый сид
        await using (var ctx1 = _factory.Create())
            await RecipeSeeder.SeedAsync(ctx1);

        // Изменяем запись напрямую
        var seedRecipe = SeedData.Recipes[0];
        await using (var modifyCtx = _factory.Create())
        {
            var recipe = await modifyCtx.Recipes.FindAsync([seedRecipe.Id]);
            Assert.NotNull(recipe);
            recipe.Update("Изменённый заголовок", null, 10, Difficulty.Easy, 1, "Шаг 1.");
            await modifyCtx.SaveChangesAsync();
        }

        // Повторный сид должен восстановить оригинальные данные
        await using (var ctx2 = _factory.Create())
            await RecipeSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var result = await readCtx.Recipes.FindAsync([seedRecipe.Id]);

        Assert.NotNull(result);
        Assert.Equal(seedRecipe.Title, result.Title);
    }
}
