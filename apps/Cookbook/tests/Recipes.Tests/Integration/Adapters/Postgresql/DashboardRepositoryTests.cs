using Microsoft.EntityFrameworkCore.Migrations;
using Recipes.Adapters.Postgresql;
using Recipes.Application.Ports;
using Recipes.Domain;
using Shared.Testing.Database;
using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[Collection("RecipeIntegration")]
public sealed class DashboardRepositoryTests(RecipeIntegrationFixture fixture) : IAsyncLifetime
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

    private static UserId NewUserId() => UserId.From(Guid.NewGuid());

    private async Task<UserId> CreateUserAsync(string? displayName = null)
    {
        var id = NewUserId();
        await using var ctx = _factory.Create();
        var user = User.Create(id, $"user-{id.Value:N}@test.local", displayName ?? $"User {id.Value:N}", "hash");
        await ctx.Users.AddAsync(user);
        await ctx.CommitAsync();
        return id;
    }

    private async Task<RecipeId> CreatePublicRecipeAsync(UserId? authorId = null, float? rating = null)
    {
        var id = RecipeId.New();
        await using var ctx = _factory.Create();
        var recipe = Recipe.Create(
            id,
            "Рецепт",
            "Описание",
            30,
            Difficulty.Easy,
            2,
            "Шаг 1.",
            isPublic: true,
            authorId: authorId);
        if (rating.HasValue)
            recipe.SetAverageRating(rating.Value);
        await ctx.Recipes.AddAsync(recipe);
        await ctx.CommitAsync();
        return id;
    }

    private async Task<CategoryId> CreateCategoryAsync(string name, CategoryType type)
    {
        var id = CategoryId.From(Guid.NewGuid());
        await using var ctx = _factory.Create();
        var category = Category.Create(id, name, string.Empty, type);
        await ctx.Categories.AddAsync(category);
        await ctx.CommitAsync();
        return id;
    }

    private async Task AssignCategoryAsync(RecipeId recipeId, CategoryId categoryId, CategoryType type)
    {
        await using var ctx = _factory.Create();
        var recipe = await ctx.Recipes.FindAsync(recipeId);
        recipe!.Update(
            recipe.Title,
            recipe.Description,
            recipe.CookingTime,
            recipe.Difficulty,
            recipe.Servings,
            recipe.Instructions,
            recipe.IsPublic,
            null,
            new Dictionary<CategoryId, CategoryType> { [categoryId] = type });
        await ctx.CommitAsync();
    }

    private async Task AddCommentAsync(RecipeId recipeId, UserId authorId)
    {
        await using var ctx = _factory.Create();
        var comment = RecipeComment.Create(RecipeCommentId.New(), recipeId, authorId, "Комментарий");
        await ctx.RecipeComments.AddAsync(comment);
        await ctx.CommitAsync();
    }

    // ── 5.1 TotalRecipes ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetTotalRecipesAsync_ReturnsCorrectCount()
    {
        await CreatePublicRecipeAsync();
        await CreatePublicRecipeAsync();

        await using var ctx = _factory.Create();
        IDashboardRepository repo = ctx;

        var total = await repo.GetTotalRecipesAsync();

        Assert.Equal(2, total);
    }

    // ── 5.2 MyRecipes / MyComments ───────────────────────────────────────────

    [Fact]
    public async Task GetMyRecipesAsync_GetMyCommentsAsync_ReturnsCorrectCounts()
    {
        var userId = await CreateUserAsync();
        var recipeId1 = await CreatePublicRecipeAsync(authorId: userId);
        var recipeId2 = await CreatePublicRecipeAsync(authorId: userId);
        await CreatePublicRecipeAsync(); // чужой рецепт

        await AddCommentAsync(recipeId1, userId);
        await AddCommentAsync(recipeId2, userId);

        await using var ctx = _factory.Create();
        IDashboardRepository repo = ctx;

        var myRecipes = await repo.GetMyRecipesAsync(userId);
        var myComments = await repo.GetMyCommentsAsync(userId);

        Assert.Equal(2, myRecipes);
        Assert.Equal(2, myComments);
    }

    // ── 5.3 TotalUsers / TotalComments ───────────────────────────────────────

    [Fact]
    public async Task GetTotalUsersAsync_GetTotalCommentsAsync_ReturnsCorrectCounts()
    {
        var u1 = await CreateUserAsync();
        var u2 = await CreateUserAsync();
        var recipeId = await CreatePublicRecipeAsync();
        await AddCommentAsync(recipeId, u1);
        await AddCommentAsync(recipeId, u2);

        await using var ctx = _factory.Create();
        IDashboardRepository repo = ctx;

        var totalUsers = await repo.GetTotalUsersAsync();
        var totalComments = await repo.GetTotalCommentsAsync();

        Assert.Equal(2, totalUsers);
        Assert.Equal(2, totalComments);
    }

    // ── 5.4 Top10ByRating ────────────────────────────────────────────────────

    [Fact]
    public async Task GetTop10ByRatingAsync_ReturnsSortedByRatingDesc()
    {
        await CreatePublicRecipeAsync(rating: 3.0f);
        await CreatePublicRecipeAsync(rating: 5.0f);
        await CreatePublicRecipeAsync(rating: 4.0f);

        await using var ctx = _factory.Create();
        IDashboardRepository repo = ctx;

        var top = await repo.GetTop10ByRatingAsync();

        Assert.Equal(3, top.Count);
        Assert.Equal(5.0f, top[0].AverageRating);
        Assert.Equal(4.0f, top[1].AverageRating);
        Assert.Equal(3.0f, top[2].AverageRating);
    }

    // ── 5.5 ByMainIngredient ─────────────────────────────────────────────────

    [Fact]
    public async Task GetByMainIngredientAsync_ReturnsCorrectGrouping()
    {
        var catMeat = await CreateCategoryAsync("Мясо", CategoryType.MainIngredient);
        var catVeg = await CreateCategoryAsync("Овощи", CategoryType.MainIngredient);

        var r1 = await CreatePublicRecipeAsync();
        var r2 = await CreatePublicRecipeAsync();
        var r3 = await CreatePublicRecipeAsync();

        await AssignCategoryAsync(r1, catMeat, CategoryType.MainIngredient);
        await AssignCategoryAsync(r2, catMeat, CategoryType.MainIngredient);
        await AssignCategoryAsync(r3, catVeg, CategoryType.MainIngredient);

        await using var ctx = _factory.Create();
        IDashboardRepository repo = ctx;

        var result = await repo.GetByMainIngredientAsync();

        Assert.NotEmpty(result);
        var meat = result.FirstOrDefault(r => r.CategoryName == "Мясо");
        var veg = result.FirstOrDefault(r => r.CategoryName == "Овощи");
        Assert.NotNull(meat);
        Assert.NotNull(veg);
        Assert.Equal(2, meat.RecipeCount);
        Assert.Equal(1, veg.RecipeCount);
        // Сортировка по убыванию
        Assert.True(result[0].RecipeCount >= result[1].RecipeCount);
    }

    // ── 5.6 PlanFill ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPlanFillAsync_ReturnsCorrectSlotFill()
    {
        var userId = await CreateUserAsync();
        var recipeId = await CreatePublicRecipeAsync();

        // Создаём план с одним заполненным слотом (Monday/Breakfast)
        await using (var ctx = _factory.Create())
        {
            var plan = MealPlan.Create(MealPlanId.New(), userId);
            plan.SetSlots([
                new MealPlanSlotInput(
                    WeekDay.Monday,
                    MealType.Breakfast,
                    [new MealPlanItemInput(recipeId, Servings.From(2))])
            ]);
            await ctx.MealPlans.AddAsync(plan);
            await ctx.CommitAsync();
        }

        await using var readCtx = _factory.Create();
        IDashboardRepository repo = readCtx;

        var planFill = await repo.GetPlanFillAsync(userId);

        Assert.NotNull(planFill);
        Assert.Equal(21, planFill.Count); // 7 дней × 3 типа
        Assert.True(planFill["Monday_Breakfast"]);
        Assert.False(planFill["Monday_Lunch"]);
        Assert.False(planFill["Sunday_Dinner"]);
    }
}
