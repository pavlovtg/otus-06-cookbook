using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeService
{
    Task<PagedResult<Recipe>> GetRecipesPagedAsync(int page, int pageSize, string? q = null, RecipeSortOrder sort = RecipeSortOrder.TitleAsc, CancellationToken cancellationToken = default);
    Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<RecipeWithIngredientDetails> GetByIdWithDetailsAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<Recipe> CreateAsync(string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, IEnumerable<RecipeIngredientInput> ingredients, IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default);
    Task UpdateAsync(RecipeId id, string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, IEnumerable<RecipeIngredientInput> ingredients, IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default);
}
