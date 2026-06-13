using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class IngredientTitleTooLongException : IngredientDomainException
{
    public int ActualLength { get; }
    public int MaxLength { get; } = IngredientConstraints.TitleMaxLength;

    public IngredientTitleTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
