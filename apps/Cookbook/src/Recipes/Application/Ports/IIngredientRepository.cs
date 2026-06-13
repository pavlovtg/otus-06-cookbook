using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IIngredientRepository : IUnitOfWorkRepository
{
    Task<PagedResult<Ingredient>> GetIngredientsAsync(
        int page,
        int pageSize,
        string? titleFilter = null,
        IngredientCategory? categoryFilter = null,
        CancellationToken cancellationToken = default);

    Task<Ingredient?> GetByIdAsync(IngredientId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
    Task DeleteAsync(IngredientId id, CancellationToken cancellationToken = default);
}
