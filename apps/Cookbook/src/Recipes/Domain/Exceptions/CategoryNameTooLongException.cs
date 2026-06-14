namespace Recipes.Domain.Exceptions;

internal sealed class CategoryNameTooLongException : CategoryDomainException
{
    public int ActualLength { get; }
    public int MaxLength { get; } = CategoryConstraints.NameMaxLength;

    public CategoryNameTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
