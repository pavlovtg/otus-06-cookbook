using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

public sealed class IngredientSeederTests : IAsyncLifetime
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
    public async Task SeedAsync_EmptyDb_InsertsAllSeedIngredients()
    {
        await using var seedCtx = _factory.Create();
        await IngredientSeeder.SeedAsync(seedCtx);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Ingredients.CountAsync();

        Assert.Equal(SeedData.Ingredients.Length, count);
    }

    [Fact]
    public async Task SeedAsync_SeedData_MatchesExpected()
    {
        await using var seedCtx = _factory.Create();
        await IngredientSeeder.SeedAsync(seedCtx);

        var expected = SeedData.Ingredients[0];

        await using var readCtx = _factory.Create();
        var actual = await readCtx.Ingredients.FindAsync([expected.Id]);

        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Unit, actual.Unit);
        Assert.Equal(expected.DefaultAmount, actual.DefaultAmount);
        Assert.Equal(expected.Category, actual.Category);
        Assert.Equal(expected.IsSystem, actual.IsSystem);
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicate()
    {
        await using (var ctx1 = _factory.Create())
            await IngredientSeeder.SeedAsync(ctx1);

        await using (var ctx2 = _factory.Create())
            await IngredientSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var count = await readCtx.Ingredients.CountAsync();

        Assert.Equal(SeedData.Ingredients.Length, count);
    }

    [Fact]
    public async Task SeedAsync_ExistingRecord_UpdatesIt()
    {
        // Первый сид
        await using (var ctx1 = _factory.Create())
            await IngredientSeeder.SeedAsync(ctx1);

        // Изменяем запись напрямую
        var seedIngredient = SeedData.Ingredients[0];
        await using (var modifyCtx = _factory.Create())
        {
            var ingredient = await modifyCtx.Ingredients.FindAsync([seedIngredient.Id]);
            Assert.NotNull(ingredient);
            ingredient.Update("Изменённое название", "шт.", 1f, IngredientCategory.Other);
            await modifyCtx.SaveChangesAsync();
        }

        // Повторный сид должен восстановить оригинальные данные
        await using (var ctx2 = _factory.Create())
            await IngredientSeeder.SeedAsync(ctx2);

        await using var readCtx = _factory.Create();
        var result = await readCtx.Ingredients.FindAsync([seedIngredient.Id]);

        Assert.NotNull(result);
        Assert.Equal(seedIngredient.Title, result.Title);
        Assert.Equal(seedIngredient.Unit, result.Unit);
        Assert.Equal(seedIngredient.DefaultAmount, result.DefaultAmount);
        Assert.Equal(seedIngredient.Category, result.Category);
    }
}
