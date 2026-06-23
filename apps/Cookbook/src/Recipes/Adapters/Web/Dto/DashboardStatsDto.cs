namespace Recipes.Adapters.Web.Dto;

internal sealed record DashboardStatsDto(
    int TotalRecipes,
    int? MyRecipes,
    int? MyComments,
    IReadOnlyList<RecipeRankDto> Top10ByRating,
    IReadOnlyList<RecipeRankDto> TopFavoritesByRating,
    IReadOnlyList<CategoryCountDto> ByMainIngredient,
    IReadOnlyList<CategoryCountDto> ByCuisine,
    int? TotalUsers,
    int? TotalComments,
    IReadOnlyList<UserRankDto>? TopUsersByRating,
    IReadOnlyList<UserRankDto>? TopUsersByComments,
    IReadOnlyDictionary<string, bool>? PlanFill);
