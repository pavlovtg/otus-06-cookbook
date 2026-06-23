namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanItemRequest(Guid RecipeId, int Servings);
