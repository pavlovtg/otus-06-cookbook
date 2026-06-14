using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<Category> CreateAsync(
        string name,
        string description,
        CategoryType type,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        CategoryId id,
        string name,
        string description,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default);
}
