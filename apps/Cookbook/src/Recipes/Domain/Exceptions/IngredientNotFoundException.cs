namespace Recipes.Domain.Exceptions;

internal sealed class IngredientNotFoundException : IngredientDomainException
{
    public IngredientId Id { get; }

    public IngredientNotFoundException(IngredientId id)
    {
        Id = id;
    }
}
