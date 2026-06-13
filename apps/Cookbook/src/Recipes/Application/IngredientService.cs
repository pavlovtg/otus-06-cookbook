using System.Runtime.CompilerServices;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _repository;

    public IngredientService(IIngredientRepository repository)
    {
        _repository = repository;
    }

    public async IAsyncEnumerable<Ingredient> GetIngredientsAsync(
        string? titleFilter = null,
        IngredientCategory? categoryFilter = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var ingredient in _repository.GetIngredientsAsync(titleFilter, categoryFilter, cancellationToken))
            yield return ingredient;
    }

    public async Task<Ingredient> GetByIdAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new IngredientNotFoundException(id);
    }

    public async Task<Ingredient> CreateAsync(
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        CancellationToken cancellationToken = default)
    {
        var ingredient = Ingredient.Create(IngredientId.New(), title, unit, defaultAmount, category);
        await _repository.CreateAsync(ingredient, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
        return ingredient;
    }

    public async Task UpdateAsync(
        IngredientId id,
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new IngredientNotFoundException(id);

        ingredient.Update(title, unit, defaultAmount, category);
        await _repository.UpdateAsync(ingredient, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        var ingredient = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new IngredientNotFoundException(id);

        await _repository.DeleteAsync(ingredient.Id, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }
}
