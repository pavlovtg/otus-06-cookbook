namespace Recipes.Application;

internal sealed record MealPlanView(
    Guid Id,
    IReadOnlyList<MealPlanSlotView> Slots);
