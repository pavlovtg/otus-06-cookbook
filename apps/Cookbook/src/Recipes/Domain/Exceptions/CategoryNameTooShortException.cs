namespace Recipes.Domain.Exceptions;

internal sealed class CategoryNameTooShortException : CategoryDomainException
{
    public int ActualLength { get; }
    public int MinLength { get; } = CategoryConstraints.NameMinLength;

    public CategoryNameTooShortException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
