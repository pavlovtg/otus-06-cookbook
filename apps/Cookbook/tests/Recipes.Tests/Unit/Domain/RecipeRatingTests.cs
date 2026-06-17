using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeRatingTests
{
    private static readonly UserId SomeUserId = UserId.From(Guid.NewGuid());
    private static readonly RecipeId SomeRecipeId = RecipeId.New();

    // ── 8.1 ──────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_WithValidValue_DoesNotThrow(int value)
    {
        var rating = RecipeRating.Create(SomeUserId, SomeRecipeId, value);

        Assert.Equal(value, rating.Value);
    }

    // ── 8.2 ──────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_WithOutOfRangeValue_ThrowsRatingValueOutOfRangeException(int value)
    {
        var ex = Assert.Throws<RatingValueOutOfRangeException>(() =>
            RecipeRating.Create(SomeUserId, SomeRecipeId, value));

        Assert.Equal(value, ex.Value);
    }

    // ── 8.3 ──────────────────────────────────────────────────────────────────

    [Fact]
    public void SetAverageRating_WithValue_UpdatesAverageRating()
    {
        var recipe = Recipe.Create(RecipeId.New(), "Борщ", null, 30, Difficulty.Everyday, 2, "Шаг 1.");

        recipe.SetAverageRating(3.5f);

        Assert.Equal(3.5f, recipe.AverageRating);
    }

    [Fact]
    public void SetAverageRating_WithNull_SetsAverageRatingToNull()
    {
        var recipe = Recipe.Create(RecipeId.New(), "Борщ", null, 30, Difficulty.Everyday, 2, "Шаг 1.");
        recipe.SetAverageRating(4f);

        recipe.SetAverageRating(null);

        Assert.Null(recipe.AverageRating);
    }
}
