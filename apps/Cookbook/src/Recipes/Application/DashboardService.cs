using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Application;

internal sealed class DashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardStatsView> GetStatsAsync(
        UserId? userId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        var totalRecipes = await _repository.GetTotalRecipesAsync(ct);

        int? myRecipes = null;
        int? myComments = null;
        IReadOnlyList<RecipeRankView> topFavoritesByRating = [];
        IReadOnlyDictionary<string, bool>? planFill = null;

        if (userId.HasValue)
        {
            myRecipes = await _repository.GetMyRecipesAsync(userId.Value, ct);
            myComments = await _repository.GetMyCommentsAsync(userId.Value, ct);
            topFavoritesByRating = await _repository.GetTopFavoritesByRatingAsync(userId.Value, ct);
            planFill = await _repository.GetPlanFillAsync(userId.Value, ct);
        }

        var top10ByRating = await _repository.GetTop10ByRatingAsync(ct);
        var byMainIngredient = await _repository.GetByMainIngredientAsync(ct);
        var byCuisine = await _repository.GetByCuisineAsync(ct);

        int? totalUsers = null;
        int? totalComments = null;
        IReadOnlyList<UserRankView>? topUsersByRating = null;
        IReadOnlyList<UserRankView>? topUsersByComments = null;

        if (isAdmin)
        {
            totalUsers = await _repository.GetTotalUsersAsync(ct);
            totalComments = await _repository.GetTotalCommentsAsync(ct);
            topUsersByRating = await _repository.GetTopUsersByRatingAsync(ct);
            topUsersByComments = await _repository.GetTopUsersByCommentsAsync(ct);
        }

        return new DashboardStatsView(
            TotalRecipes: totalRecipes,
            MyRecipes: myRecipes,
            MyComments: myComments,
            Top10ByRating: top10ByRating,
            TopFavoritesByRating: topFavoritesByRating,
            ByMainIngredient: byMainIngredient,
            ByCuisine: byCuisine,
            TotalUsers: totalUsers,
            TotalComments: totalComments,
            TopUsersByRating: topUsersByRating,
            TopUsersByComments: topUsersByComments,
            PlanFill: planFill);
    }
}
