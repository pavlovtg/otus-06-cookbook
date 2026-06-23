using Recipes.Domain;

namespace Recipes.Application;

internal sealed record ShoppingListGroupView(
    IngredientCategory Category,
    IReadOnlyList<ShoppingListItemView> Items);
