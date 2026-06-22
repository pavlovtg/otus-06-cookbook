namespace Recipes.Domain;

internal readonly record struct RecipeCommentId(Guid Value)
{
    public static RecipeCommentId New() => new(Guid.NewGuid());

    public static RecipeCommentId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RecipeCommentId cannot be empty.", nameof(value));

        return new RecipeCommentId(value);
    }

    public override string ToString() => Value.ToString("D");
}
