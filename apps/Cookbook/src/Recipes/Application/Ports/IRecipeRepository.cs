using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeRepository : IUnitOfWorkRepository
{
    Task<PagedResult<Recipe>> GetRecipesPagedAsync(int page, int pageSize, string? q = null, RecipeSortOrder sort = RecipeSortOrder.TitleAsc, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeWithIngredientDetails?> GetByIdWithDetailsAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeUsageResult> GetRecipesUsingIngredientAsync(IngredientId ingredientId, CancellationToken cancellationToken = default);
}
