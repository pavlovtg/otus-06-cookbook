using Recipes.Domain;

namespace Recipes.Application;

internal sealed record ShoppingListItemView(
    IngredientId IngredientId,
    string Title,
    decimal Amount,
    string Unit);
