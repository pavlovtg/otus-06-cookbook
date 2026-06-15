namespace Recipes.Domain;

internal sealed class RecipeCategory
{
    public RecipeId RecipeId { get; private set; }
    public CategoryId CategoryId { get; private set; }

    private RecipeCategory() { }

    public static RecipeCategory Create(RecipeId recipeId, CategoryId categoryId) =>
        new() { RecipeId = recipeId, CategoryId = categoryId };
}
