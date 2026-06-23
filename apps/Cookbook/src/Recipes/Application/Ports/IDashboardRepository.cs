using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IDashboardRepository
{
    Task<int> GetTotalRecipesAsync(CancellationToken ct = default);
    Task<int> GetMyRecipesAsync(UserId userId, CancellationToken ct = default);
    Task<int> GetMyCommentsAsync(UserId userId, CancellationToken ct = default);
    Task<IReadOnlyList<RecipeRankView>> GetTop10ByRatingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RecipeRankView>> GetTopFavoritesByRatingAsync(UserId userId, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryCountView>> GetByMainIngredientAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CategoryCountView>> GetByCuisineAsync(CancellationToken ct = default);
    Task<int> GetTotalUsersAsync(CancellationToken ct = default);
    Task<int> GetTotalCommentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UserRankView>> GetTopUsersByRatingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UserRankView>> GetTopUsersByCommentsAsync(CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, bool>> GetPlanFillAsync(UserId userId, CancellationToken ct = default);
}
