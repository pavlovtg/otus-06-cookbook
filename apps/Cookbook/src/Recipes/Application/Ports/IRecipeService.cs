using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeService
{
    IAsyncEnumerable<Recipe> GetAllAsync(CancellationToken cancellationToken = default);
}
