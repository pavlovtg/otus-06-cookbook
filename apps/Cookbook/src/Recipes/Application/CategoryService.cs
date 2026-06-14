using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetCategoriesAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(
        string name,
        string description,
        CategoryType type,
        CancellationToken cancellationToken = default)
    {
        var count = await _repository.CountAsync(cancellationToken);
        if (count >= CategoryConstraints.MaxCount)
            throw new CategoryLimitExceededException();

        var category = Category.Create(CategoryId.New(), name, description, type);
        await _repository.CreateAsync(category, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
        return category;
    }

    public async Task UpdateAsync(
        CategoryId id,
        string name,
        string description,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CategoryNotFoundException(id);

        category.Update(name, description);
        await _repository.UpdateAsync(category, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CategoryNotFoundException(id);

        var isUsed = await _repository.IsUsedInRecipesAsync(id, cancellationToken);
        if (isUsed)
            throw new CategoryInUseException(id);

        await _repository.DeleteAsync(category.Id, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }
}
