using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class ShoppingListRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<UserId> CreateUserAsync()
    {
        var id = UserId.New();
        await using var ctx = _factory.Create();
        var user = User.Create(id, $"user-{id.Value:N}@test.local", $"Test {id.Value:N}", "hash");
        await ctx.Users.AddAsync(user);
        await ctx.CommitAsync();
        return id;
    }

    private async Task<IngredientId> CreateIngredientAsync(
        string title,
        string unit,
        IngredientCategory category,
        float defaultAmount = 1f)
    {
        var id = IngredientId.New();
        await using var ctx = _factory.Create();
        var ingredient = Ingredient.Create(id, title, unit, defaultAmount, category);
        await ctx.Ingredients.AddAsync(ingredient);
        await ctx.CommitAsync();
        return id;
    }

    private async Task<RecipeId> CreateRecipeWithIngredientAsync(
        IngredientId ingredientId,
        decimal amount,
        int servings = 4)
    {
        var id = RecipeId.New();
        await using var ctx = _factory.Create();
        var recipe = Recipe.Create(
            id,
            $"Recipe-{id.Value:N}",
            null,
            30,
            Difficulty.Easy,
            servings,
            "Instructions.",
            [RecipeIngredient.Create(ingredientId, amount)]);
        await ctx.Recipes.AddAsync(recipe);
        await ctx.CommitAsync();
        return id;
    }

    private async Task SavePlanAsync(UserId userId, IEnumerable<MealPlanSlotInput> slots)
    {
        var plan = MealPlan.Create(MealPlanId.New(), userId);
        plan.SetSlots(slots.ToList());

        await using var ctx = _factory.Create();
        await ctx.SavePlanAsync(plan);
        await ctx.CommitAsync();
    }

    // ── 5.1 Корректная агрегация (несколько слотов, несколько рецептов) ──────

    [Fact]
    public async Task GetShoppingListAsync_MultipleSlots_AggregatesCorrectly()
    {
        var userId = await CreateUserAsync();

        // Рецепт A: 4 порции, ингредиент X = 200г
        var ingX = await CreateIngredientAsync("Flour", "g", IngredientCategory.GrainsAndCereals);
        var recipeA = await CreateRecipeWithIngredientAsync(ingX, 200m, servings: 4);

        // Рецепт B: 2 порции, ингредиент Y = 100мл
        var ingY = await CreateIngredientAsync("Milk", "ml", IngredientCategory.DairyAndEggs);
        var recipeB = await CreateRecipeWithIngredientAsync(ingY, 100m, servings: 2);

        // Слот 1: рецепт A на 4 порции → factor=1 → 200г
        // Слот 2: рецепт B на 2 порции → factor=1 → 100мл
        await SavePlanAsync(userId, [
            new MealPlanSlotInput(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(recipeA, Servings.From(4))]),
            new MealPlanSlotInput(WeekDay.Monday, MealType.Lunch,
                [new MealPlanItemInput(recipeB, Servings.From(2))]),
        ]);

        await using var ctx = _factory.Create();
        var result = await ctx.GetShoppingListAsync(userId);

        Assert.Equal(2, result.Sum(g => g.Items.Count));

        var flour = result.SelectMany(g => g.Items).Single(i => i.Title == "Flour");
        var milk = result.SelectMany(g => g.Items).Single(i => i.Title == "Milk");

        Assert.Equal(200m, flour.Amount);
        Assert.Equal(100m, milk.Amount);
    }

    // ── 5.2 Пересчёт количества при нестандартных порциях ────────────────────

    [Fact]
    public async Task GetShoppingListAsync_NonStandardServings_RecalculatesAmount()
    {
        var userId = await CreateUserAsync();

        // Рецепт на 4 порции, ингредиент = 200г
        var ing = await CreateIngredientAsync("Sugar", "g", IngredientCategory.BakeryAndSweets);
        var recipe = await CreateRecipeWithIngredientAsync(ing, 200m, servings: 4);

        // В плане — 2 порции → factor = 2/4 = 0.5 → ожидаем 100г
        await SavePlanAsync(userId, [
            new MealPlanSlotInput(WeekDay.Tuesday, MealType.Breakfast,
                [new MealPlanItemInput(recipe, Servings.From(2))]),
        ]);

        await using var ctx = _factory.Create();
        var result = await ctx.GetShoppingListAsync(userId);

        var item = result.SelectMany(g => g.Items).Single(i => i.Title == "Sugar");
        Assert.Equal(100m, item.Amount);
    }

    // ── 5.3 Дедупликация одинаковых ингредиентов из разных слотов ────────────

    [Fact]
    public async Task GetShoppingListAsync_SameIngredientInDifferentSlots_Deduplicates()
    {
        var userId = await CreateUserAsync();

        // Один ингредиент в двух рецептах
        var ing = await CreateIngredientAsync("Salt", "g", IngredientCategory.SpicesAndSeasonings);
        var recipeA = await CreateRecipeWithIngredientAsync(ing, 10m, servings: 2);
        var recipeB = await CreateRecipeWithIngredientAsync(ing, 20m, servings: 4);

        // Слот 1: рецепт A на 2 порции → factor=1 → 10г
        // Слот 2: рецепт B на 4 порции → factor=1 → 20г
        // Итого: 30г
        await SavePlanAsync(userId, [
            new MealPlanSlotInput(WeekDay.Wednesday, MealType.Breakfast,
                [new MealPlanItemInput(recipeA, Servings.From(2))]),
            new MealPlanSlotInput(WeekDay.Wednesday, MealType.Lunch,
                [new MealPlanItemInput(recipeB, Servings.From(4))]),
        ]);

        await using var ctx = _factory.Create();
        var result = await ctx.GetShoppingListAsync(userId);

        var saltItems = result.SelectMany(g => g.Items).Where(i => i.Title == "Salt").ToList();
        Assert.Single(saltItems);
        Assert.Equal(30m, saltItems[0].Amount);
    }

    // ── 5.4 Пустой план → пустой список ──────────────────────────────────────

    [Fact]
    public async Task GetShoppingListAsync_EmptyPlan_ReturnsEmptyList()
    {
        var userId = await CreateUserAsync();

        await SavePlanAsync(userId, []);

        await using var ctx = _factory.Create();
        var result = await ctx.GetShoppingListAsync(userId);

        Assert.Empty(result);
    }

    // ── 5.5 Группировка и сортировка ─────────────────────────────────────────

    [Fact]
    public async Task GetShoppingListAsync_GroupsAndSortsCorrectly()
    {
        var userId = await CreateUserAsync();

        var ingMeat = await CreateIngredientAsync("Beef", "g", IngredientCategory.MeatAndPoultry);
        var ingVeg1 = await CreateIngredientAsync("Zucchini", "g", IngredientCategory.Vegetables);
        var ingVeg2 = await CreateIngredientAsync("Carrot", "g", IngredientCategory.Vegetables);
        var ingDairy = await CreateIngredientAsync("Butter", "g", IngredientCategory.DairyAndEggs);

        var recipeId = RecipeId.New();
        var recipe = Recipe.Create(
            recipeId,
            "Mixed Recipe",
            null,
            60,
            Difficulty.Everyday,
            4,
            "Instructions.",
            [
                RecipeIngredient.Create(ingMeat, 300m),
                RecipeIngredient.Create(ingVeg1, 150m),
                RecipeIngredient.Create(ingVeg2, 100m),
                RecipeIngredient.Create(ingDairy, 50m),
            ]);

        await using (var ctx = _factory.Create())
        {
            await ctx.Recipes.AddAsync(recipe);
            await ctx.CommitAsync();
        }

        await SavePlanAsync(userId, [
            new MealPlanSlotInput(WeekDay.Thursday, MealType.Dinner,
                [new MealPlanItemInput(recipeId, Servings.From(4))]),
        ]);

        await using var readCtx = _factory.Create();
        var result = await readCtx.GetShoppingListAsync(userId);

        // Группы отсортированы по порядку enum
        var groupCategories = result.Select(g => g.Category).ToList();
        Assert.Equal(groupCategories.OrderBy(c => (int)c).ToList(), groupCategories);

        // Внутри группы Vegetables: Carrot < Zucchini (алфавитный порядок)
        var vegGroup = result.Single(g => g.Category == IngredientCategory.Vegetables);
        Assert.Equal(2, vegGroup.Items.Count);
        Assert.Equal("Carrot", vegGroup.Items[0].Title);
        Assert.Equal("Zucchini", vegGroup.Items[1].Title);
    }
}
