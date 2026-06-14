namespace Recipes.Domain.Exceptions;

internal sealed class RecipeIngredientAmountOutOfRangeException : RecipeDomainException
{
    public decimal MinValue { get; } = RecipeConstraints.IngredientAmountMin;
    public decimal MaxValue { get; } = RecipeConstraints.IngredientAmountMax;
    public decimal ActualValue { get; }

    public RecipeIngredientAmountOutOfRangeException(decimal actualValue)
    {
        ActualValue = actualValue;
    }
}
