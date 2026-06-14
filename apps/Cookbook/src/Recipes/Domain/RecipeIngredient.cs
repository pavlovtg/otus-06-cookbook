using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class RecipeIngredient
{
    public IngredientId IngredientId { get; private set; }
    public decimal Amount { get; private set; }

    private RecipeIngredient() { }

    public static RecipeIngredient Create(IngredientId ingredientId, decimal amount)
    {
        if (amount < RecipeConstraints.IngredientAmountMin || amount > RecipeConstraints.IngredientAmountMax)
            throw new RecipeIngredientAmountOutOfRangeException(amount);

        return new RecipeIngredient
        {
            IngredientId = ingredientId,
            Amount = amount,
        };
    }
}
