namespace Recipes.Application;

internal sealed record MealPlanItemView(
    Guid Id,
    Guid RecipeId,
    string RecipeTitle,
    int Servings);
