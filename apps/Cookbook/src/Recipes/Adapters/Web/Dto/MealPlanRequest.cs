namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanRequest(IReadOnlyList<MealPlanSlotRequest> Slots);
