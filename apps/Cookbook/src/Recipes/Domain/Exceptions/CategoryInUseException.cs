namespace Recipes.Domain.Exceptions;

internal sealed class CategoryInUseException : CategoryDomainException
{
    public CategoryId Id { get; }

    public CategoryInUseException(CategoryId id)
    {
        Id = id;
    }
}
