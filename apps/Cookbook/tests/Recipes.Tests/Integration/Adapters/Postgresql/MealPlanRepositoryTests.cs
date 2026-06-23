using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class MealPlanRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
{
    private RepositoryFactory<RecipeRepository> _factory = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();

        _factory = new RepositoryFactory<RecipeRepository>(
            fixture.ConnectionString,
            builder => new RecipeRepository(builder.Options),
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema));
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    private static UserId NewUserId() => UserId.From(Guid.NewGuid());
    private static RecipeId NewRecipeId() => RecipeId.From(Guid.NewGuid());

    private static MealPlan NewPlan(UserId? userId = null) =>
        MealPlan.Create(MealPlanId.New(), userId ?? NewUserId());

    private async Task<UserId> CreateUserAsync(UserId? userId = null)
    {
        var id = userId ?? NewUserId();
        await using var ctx = _factory.Create();
        var user = User.Create(
            id,
            $"user-{id.Value:N}@test.local",
            $"Test User {id.Value:N}",
            "hash");
        await ctx.Users.AddAsync(user);
        await ctx.CommitAsync();
        return id;
    }

    // ── GetPlanByUserIdAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetPlanByUserIdAsync_NonExistentUser_ReturnsNull()
    {
        await using var ctx = _factory.Create();

        var result = await ctx.GetPlanByUserIdAsync(NewUserId());

        Assert.Null(result);
    }

    // ── SavePlanAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task SavePlanAsync_NewPlan_ThenGetByUserId_ReturnsPlan()
    {
        var userId = await CreateUserAsync();
        var plan = NewPlan(userId);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetPlanByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(plan.Id, result.Id);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task SavePlanAsync_WithSlots_SlotsAndItemsPersisted()
    {
        var userId = await CreateUserAsync();
        var plan = NewPlan(userId);
        var recipeId = NewRecipeId();

        plan.SetSlots([
            new MealPlanSlotInput(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(recipeId, Servings.From(3))])
        ]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetPlanByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Single(result.Slots);
        Assert.Equal(WeekDay.Monday, result.Slots[0].WeekDay);
        Assert.Equal(MealType.Breakfast, result.Slots[0].MealType);
        Assert.Single(result.Slots[0].Items);
        Assert.Equal(recipeId, result.Slots[0].Items[0].RecipeId);
        Assert.Equal(3, result.Slots[0].Items[0].Servings.Value);
    }

    [Fact]
    public async Task SavePlanAsync_ExistingPlan_UpdatesSlots()
    {
        var userId = await CreateUserAsync();
        var plan = NewPlan(userId);

        plan.SetSlots([
            new MealPlanSlotInput(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(2))])
        ]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan);
            await writeCtx.CommitAsync();
        }

        // Обновляем слоты
        await using (var updateCtx = _factory.Create())
        {
            var loaded = await updateCtx.GetPlanByUserIdAsync(userId);
            Assert.NotNull(loaded);

            loaded.SetSlots([
                new MealPlanSlotInput(WeekDay.Wednesday, MealType.Lunch,
                    [new MealPlanItemInput(NewRecipeId(), Servings.From(5))])
            ]);

            await updateCtx.SavePlanAsync(loaded);
            await updateCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetPlanByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Single(result.Slots);
        Assert.Equal(WeekDay.Wednesday, result.Slots[0].WeekDay);
        Assert.Equal(MealType.Lunch, result.Slots[0].MealType);
        Assert.Equal(5, result.Slots[0].Items[0].Servings.Value);
    }

    [Fact]
    public async Task SavePlanAsync_ThenClearSlots_SlotsBecomeEmpty()
    {
        var userId = await CreateUserAsync();
        var plan = NewPlan(userId);

        plan.SetSlots([
            new MealPlanSlotInput(WeekDay.Thursday, MealType.Dinner,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(1))])
        ]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan);
            await writeCtx.CommitAsync();
        }

        await using (var clearCtx = _factory.Create())
        {
            var loaded = await clearCtx.GetPlanByUserIdAsync(userId);
            Assert.NotNull(loaded);

            loaded.ClearSlots();
            await clearCtx.SavePlanAsync(loaded);
            await clearCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetPlanByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Empty(result.Slots);
    }

    [Fact]
    public async Task SavePlanAsync_WithMultipleSlots_AllSlotsPersisted()
    {
        var userId = await CreateUserAsync();
        var plan = NewPlan(userId);

        plan.SetSlots([
            new MealPlanSlotInput(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(1))]),
            new MealPlanSlotInput(WeekDay.Monday, MealType.Lunch,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(2))]),
            new MealPlanSlotInput(WeekDay.Monday, MealType.Dinner,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(3))])
        ]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetPlanByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(3, result.Slots.Count);
    }

    [Fact]
    public async Task SavePlanAsync_TwoDifferentUsers_PlansAreIndependent()
    {
        var userId1 = await CreateUserAsync();
        var userId2 = await CreateUserAsync();

        var plan1 = NewPlan(userId1);
        var plan2 = NewPlan(userId2);

        plan1.SetSlots([new MealPlanSlotInput(WeekDay.Monday, MealType.Breakfast,
            [new MealPlanItemInput(NewRecipeId(), Servings.From(1))])]);

        await using (var writeCtx = _factory.Create())
        {
            await writeCtx.SavePlanAsync(plan1);
            await writeCtx.SavePlanAsync(plan2);
            await writeCtx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        var result1 = await readCtx.GetPlanByUserIdAsync(userId1);
        var result2 = await readCtx.GetPlanByUserIdAsync(userId2);

        Assert.NotNull(result1);
        Assert.Single(result1.Slots);

        Assert.NotNull(result2);
        Assert.Empty(result2.Slots);
    }
}
