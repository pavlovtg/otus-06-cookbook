using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class RecipeCookingTimeOutOfRangeException : RecipeDomainException
{
    public int MinValue { get; } = RecipeConstraints.CookingTimeMin;
    public int MaxValue { get; } = RecipeConstraints.CookingTimeMax;
    public int ActualValue { get; }

    public RecipeCookingTimeOutOfRangeException(int actualValue)
    {
        ActualValue = actualValue;
    }
}
