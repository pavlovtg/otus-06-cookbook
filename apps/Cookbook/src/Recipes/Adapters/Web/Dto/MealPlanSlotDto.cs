namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanSlotDto(
    int WeekDay,
    int MealType,
    IReadOnlyList<MealPlanItemDto> Items);
