using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class IngredientTitleTooShortException : IngredientDomainException
{
    public int ActualLength { get; }
    public int MinLength { get; } = IngredientConstraints.TitleMinLength;

    public IngredientTitleTooShortException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
