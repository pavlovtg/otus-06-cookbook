using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeRepository : IUnitOfWorkRepository
{
    IAsyncEnumerable<Recipe> GetRecipesAsync(CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default);
}
