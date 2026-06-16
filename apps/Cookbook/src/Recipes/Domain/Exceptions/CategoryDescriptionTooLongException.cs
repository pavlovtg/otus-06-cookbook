namespace Recipes.Domain.Exceptions;

internal sealed class CategoryDescriptionTooLongException : CategoryDomainException
{
    public int ActualLength { get; }
    public int MaxLength { get; } = CategoryConstraints.DescriptionMaxLength;

    public CategoryDescriptionTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
