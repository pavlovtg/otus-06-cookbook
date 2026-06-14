using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface ICategoryRepository : IUnitOfWorkRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<bool> IsUsedInRecipesAsync(CategoryId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default);
}
