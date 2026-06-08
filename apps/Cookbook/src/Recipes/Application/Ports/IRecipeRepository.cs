using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeRepository
{
    IAsyncEnumerable<Recipe> GetAllAsync(CancellationToken cancellationToken = default);
}
