using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IShoppingListRepository
{
    Task<IReadOnlyList<ShoppingListGroupView>> GetShoppingListAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
