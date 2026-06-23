namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanSlotRequest(
    int WeekDay,
    int MealType,
    IReadOnlyList<MealPlanItemRequest> Items);
