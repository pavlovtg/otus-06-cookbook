namespace Recipes.Domain.Exceptions;

internal sealed class RatingValueOutOfRangeException(int value) : RecipeDomainException
{
    public int Value { get; } = value;
}
