namespace Recipes.Domain;

internal readonly record struct RecipePhotoId(Guid Value)
{
    public static RecipePhotoId New() => new(Guid.NewGuid());

    public static RecipePhotoId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RecipePhotoId cannot be empty.", nameof(value));

        return new RecipePhotoId(value);
    }

    public override string ToString() => Value.ToString("D");
}
