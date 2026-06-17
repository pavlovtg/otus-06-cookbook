using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeService
{
    Task<PagedResult<RecipeShortWithAuthor>> GetRecipesPagedAsync(int page, int pageSize, string? q = null, RecipeSortOrder sort = RecipeSortOrder.TitleAsc, UserId? currentUserId = null, bool? favorites = null, CancellationToken cancellationToken = default);
    Task AddFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
    Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeWithIngredientDetails> GetByIdWithDetailsAsync(RecipeId id, UserId? currentUserId = null, CancellationToken cancellationToken = default);
    Task<Recipe> CreateAsync(string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, bool isPublic, UserId? authorId, IEnumerable<RecipeIngredientInput> ingredients, IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default);
    Task UpdateAsync(RecipeId id, string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, bool isPublic, UserId? currentUserId, UserRole? currentUserRole, IEnumerable<RecipeIngredientInput> ingredients, IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, UserId? currentUserId, UserRole? currentUserRole, CancellationToken cancellationToken = default);
    Task<RatingSummary> SetRatingAsync(UserId userId, RecipeId recipeId, int value, CancellationToken cancellationToken = default);
    Task DeleteRatingAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);
}
