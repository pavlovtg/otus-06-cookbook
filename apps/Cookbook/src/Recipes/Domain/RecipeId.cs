namespace Recipes.Domain;

internal readonly record struct RecipeId(Guid Value)
{
    public static RecipeId New() => new(Guid.NewGuid());

    public static RecipeId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RecipeId cannot be empty.", nameof(value));

        return new RecipeId(value);
    }

    public override string ToString() => Value.ToString("D");
}
