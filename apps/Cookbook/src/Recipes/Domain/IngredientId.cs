namespace Recipes.Domain;

internal readonly record struct IngredientId(Guid Value)
{
    public static IngredientId New() => new(Guid.NewGuid());

    public static IngredientId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("IngredientId cannot be empty.", nameof(value));

        return new IngredientId(value);
    }

    public override string ToString() => Value.ToString("D");
}
