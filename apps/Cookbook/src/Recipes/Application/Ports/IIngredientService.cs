using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IIngredientService
{
    IAsyncEnumerable<Ingredient> GetAllAsync(
        string? titleFilter = null,
        IngredientCategory? categoryFilter = null,
        CancellationToken cancellationToken = default);

    Task<Ingredient> GetByIdAsync(IngredientId id, CancellationToken cancellationToken = default);

    Task<Ingredient> CreateAsync(
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        IngredientId id,
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(IngredientId id, CancellationToken cancellationToken = default);
}
