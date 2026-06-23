namespace Recipes.Adapters.Web.Dto;

internal sealed record MealPlanDto(
    Guid Id,
    IReadOnlyList<MealPlanSlotDto> Slots);
