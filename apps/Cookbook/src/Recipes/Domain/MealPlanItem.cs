namespace Recipes.Domain;

internal sealed class MealPlanItem
{
    public Guid Id { get; private set; }
    public RecipeId RecipeId { get; private set; }
    public Servings Servings { get; private set; }

    private MealPlanItem() { }

    public static MealPlanItem Create(RecipeId recipeId, Servings servings)
    {
        return new MealPlanItem
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            Servings = servings,
        };
    }

    internal void UpdateServings(Servings servings)
    {
        Servings = servings;
    }
}
