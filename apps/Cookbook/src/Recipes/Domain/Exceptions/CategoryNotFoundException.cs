namespace Recipes.Domain.Exceptions;

internal sealed class CategoryNotFoundException : CategoryDomainException
{
    public CategoryId Id { get; }

    public CategoryNotFoundException(CategoryId id)
    {
        Id = id;
    }
}
