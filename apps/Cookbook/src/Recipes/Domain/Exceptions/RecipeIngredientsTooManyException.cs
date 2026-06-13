namespace Recipes.Domain.Exceptions;

internal sealed class RecipeIngredientsTooManyException : RecipeDomainException
{
    public int MaxCount { get; } = RecipeConstraints.IngredientsMaxCount;
    public int ActualCount { get; }

    public RecipeIngredientsTooManyException(int actualCount)
    {
        ActualCount = actualCount;
    }
}
