using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeIngredientTests
{
    private static readonly IngredientId ValidIngredientId = IngredientId.New();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.001)]
    public void Create_WithAmountZeroOrNegative_ThrowsRecipeIngredientAmountOutOfRangeException(decimal amount)
    {
        var ex = Assert.Throws<RecipeIngredientAmountOutOfRangeException>(() =>
            RecipeIngredient.Create(ValidIngredientId, amount));

        Assert.Equal(amount, ex.ActualValue);
    }

    [Fact]
    public void Create_WithAmountAboveMax_ThrowsRecipeIngredientAmountOutOfRangeException()
    {
        const decimal amount = 100_001m;

        var ex = Assert.Throws<RecipeIngredientAmountOutOfRangeException>(() =>
            RecipeIngredient.Create(ValidIngredientId, amount));

        Assert.Equal(amount, ex.ActualValue);
    }

    [Theory]
    [InlineData("0.001")]
    [InlineData("100000")]
    public void Create_WithBoundaryAmount_Succeeds(string amountStr)
    {
        var amount = decimal.Parse(amountStr);

        var ingredient = RecipeIngredient.Create(ValidIngredientId, amount);

        Assert.Equal(ValidIngredientId, ingredient.IngredientId);
        Assert.Equal(amount, ingredient.Amount);
    }

    [Fact]
    public void Create_WithValidAmount_SetsIngredientId()
    {
        var id = IngredientId.New();
        const decimal amount = 250m;

        var ingredient = RecipeIngredient.Create(id, amount);

        Assert.Equal(id, ingredient.IngredientId);
        Assert.Equal(amount, ingredient.Amount);
    }
}
