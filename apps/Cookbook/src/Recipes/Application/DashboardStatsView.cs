namespace Recipes.Application;

internal sealed record DashboardStatsView(
    int TotalRecipes,
    int? MyRecipes,
    int? MyComments,
    IReadOnlyList<RecipeRankView> Top10ByRating,
    IReadOnlyList<RecipeRankView> TopFavoritesByRating,
    IReadOnlyList<CategoryCountView> ByMainIngredient,
    IReadOnlyList<CategoryCountView> ByCuisine,
    int? TotalUsers,
    int? TotalComments,
    IReadOnlyList<UserRankView>? TopUsersByRating,
    IReadOnlyList<UserRankView>? TopUsersByComments,
    IReadOnlyDictionary<string, bool>? PlanFill);
