namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanItemDto(Guid Id, Guid RecipeId, string RecipeTitle, int Servings);
