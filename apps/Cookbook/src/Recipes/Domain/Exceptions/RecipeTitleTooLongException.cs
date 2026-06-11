using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class RecipeTitleTooLongException : RecipeDomainException
{
    public int MaxLength { get; } = RecipeConstraints.TitleMaxLength;
    public int ActualLength { get; }

    public RecipeTitleTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
