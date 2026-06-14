namespace Recipes.Domain.Exceptions;

internal sealed class RecipePhotoNotFoundException : RecipeDomainException
{
    public RecipePhotoId Id { get; }

    public RecipePhotoNotFoundException(RecipePhotoId id)
    {
        Id = id;
    }
}
