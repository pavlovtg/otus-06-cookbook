using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class RecipeDescriptionTooLongException : RecipeDomainException
{
    public int MaxLength { get; } = RecipeConstraints.DescriptionMaxLength;
    public int ActualLength { get; }

    public RecipeDescriptionTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
