namespace Recipes.Domain.Exceptions;

internal sealed class IngredientDefaultAmountOutOfRangeException : IngredientDomainException
{
    public float ActualValue { get; }

    public IngredientDefaultAmountOutOfRangeException(float actualValue)
    {
        ActualValue = actualValue;
    }
}
