using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Application;

internal sealed class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public IAsyncEnumerable<Recipe> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
