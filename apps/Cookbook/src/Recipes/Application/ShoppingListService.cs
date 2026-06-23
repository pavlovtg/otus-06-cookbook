using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Application;

internal sealed class ShoppingListService
{
    private readonly IShoppingListRepository _repository;

    public ShoppingListService(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ShoppingListGroupView>> GetAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetShoppingListAsync(userId, cancellationToken);
    }
}
