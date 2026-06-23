namespace Recipes.Adapters.Web.Dto;

internal sealed record ShoppingListItemDto(
    Guid IngredientId,
    string Title,
    decimal Amount,
    string Unit);
