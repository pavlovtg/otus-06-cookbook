namespace Recipes.Domain;

internal sealed record MealPlanSlotInput(
    WeekDay WeekDay,
    MealType MealType,
    IReadOnlyList<MealPlanItemInput> Items);
