namespace Recipes.Domain.Exceptions;

internal sealed class RecipeDuplicateCategoryTypeException : RecipeDomainException
{
    public CategoryType DuplicateType { get; }

    public RecipeDuplicateCategoryTypeException(CategoryType duplicateType)
    {
        DuplicateType = duplicateType;
    }
}
