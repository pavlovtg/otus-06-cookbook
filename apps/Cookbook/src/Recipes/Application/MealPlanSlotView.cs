using Recipes.Domain;

namespace Recipes.Application;

internal sealed record MealPlanSlotView(
    WeekDay WeekDay,
    MealType MealType,
    IReadOnlyList<MealPlanItemView> Items);
