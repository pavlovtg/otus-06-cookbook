namespace Recipes.Domain;

internal sealed class UserFavorite
{
    public UserId UserId { get; private set; }
    public RecipeId RecipeId { get; private set; }

    private UserFavorite() { }

    public static UserFavorite Create(UserId userId, RecipeId recipeId) =>
        new() { UserId = userId, RecipeId = recipeId };
}
