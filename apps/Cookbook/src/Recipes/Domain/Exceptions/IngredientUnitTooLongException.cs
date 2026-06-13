using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class IngredientUnitTooLongException : IngredientDomainException
{
    public int ActualLength { get; }
    public int MaxLength { get; } = IngredientConstraints.UnitMaxLength;

    public IngredientUnitTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
