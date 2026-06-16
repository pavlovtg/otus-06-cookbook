namespace Recipes.Domain.Exceptions;

internal sealed class CategoryLimitExceededException : CategoryDomainException
{
    public int Limit { get; } = CategoryConstraints.MaxCount;
}
