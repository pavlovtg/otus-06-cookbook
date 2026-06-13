namespace Recipes.Domain.Exceptions;

internal sealed class RecipeNotFoundException : RecipeDomainException
{
    public RecipeId Id { get; }

    public RecipeNotFoundException(RecipeId id)
    {
        Id = id;
    }
}
