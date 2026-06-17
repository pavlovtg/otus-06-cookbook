using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeRepository : IUnitOfWorkRepository
{
    Task<PagedResult<RecipeListItem>> GetRecipesPagedAsync(int page, int pageSize, string? q = null, RecipeSortOrder sort = RecipeSortOrder.TitleAsc, UserId? currentUserId = null, bool? favorites = null, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeWithIngredientDetails?> GetByIdWithDetailsAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeUsageResult> GetRecipesUsingIngredientAsync(IngredientId ingredientId, CancellationToken cancellationToken = default);
    Task AddFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<RecipeId>> GetFavoriteIdsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task UpsertRatingAsync(UserId userId, RecipeId recipeId, int value, CancellationToken cancellationToken = default);
    Task<bool> DeleteRatingAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
    Task<float?> GetAverageRatingAsync(RecipeId recipeId, CancellationToken cancellationToken = default);
    Task<int?> GetMyRatingAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
}
