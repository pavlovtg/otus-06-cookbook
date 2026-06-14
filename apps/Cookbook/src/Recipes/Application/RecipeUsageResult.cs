namespace Recipes.Application;

internal sealed record RecipeUsageResult(
    IReadOnlyList<string> TopTitles,
    int TotalCount);
