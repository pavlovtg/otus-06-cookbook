using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipeRepository : IRecipeRepository
{
    private readonly RecipesDbContext _dbContext;

    public RecipeRepository(RecipesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<Recipe> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var recipe in _dbContext.Recipes
            .AsNoTracking()
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            yield return recipe;
        }
    }
}
