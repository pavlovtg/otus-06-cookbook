namespace Recipes.Domain;

internal sealed class Recipe
{
    public RecipeId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Recipe() { }

    public static Recipe Create(RecipeId id, string title, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return new Recipe
        {
            Id = id,
            Title = title,
            Description = description,
        };
    }
}
