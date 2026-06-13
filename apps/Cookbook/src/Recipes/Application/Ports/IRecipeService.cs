using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeService
{
    IAsyncEnumerable<Recipe> GetRecipesAsync(CancellationToken cancellationToken = default);
    Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task<Recipe> CreateAsync(string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, CancellationToken cancellationToken = default);
    Task UpdateAsync(RecipeId id, string title, string? description, int cookingTime, Difficulty difficulty, int servings, string instructions, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default);
}
