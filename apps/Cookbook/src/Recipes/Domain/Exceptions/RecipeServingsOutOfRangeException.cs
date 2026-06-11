using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class RecipeServingsOutOfRangeException : RecipeDomainException
{
    public int MinValue { get; } = RecipeConstraints.ServingsMin;
    public int MaxValue { get; } = RecipeConstraints.ServingsMax;
    public int ActualValue { get; }

    public RecipeServingsOutOfRangeException(int actualValue)
    {
        ActualValue = actualValue;
    }
}
