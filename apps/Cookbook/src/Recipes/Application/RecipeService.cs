using System.Runtime.CompilerServices;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async IAsyncEnumerable<Recipe> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var recipe in _repository.GetAllAsync(cancellationToken))
            yield return recipe;
    }

    public async Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);
    }

    public async Task<Recipe> CreateAsync(
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        CancellationToken cancellationToken = default)
    {
        var recipe = Recipe.Create(RecipeId.New(), title, description, cookingTime, difficulty, servings, instructions);
        await _repository.CreateAsync(recipe, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
        return recipe;
    }

    public async Task UpdateAsync(
        RecipeId id,
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        recipe.Update(title, description, cookingTime, difficulty, servings, instructions);
        await _repository.UpdateAsync(recipe, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        await _repository.DeleteAsync(recipe.Id, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }
}
