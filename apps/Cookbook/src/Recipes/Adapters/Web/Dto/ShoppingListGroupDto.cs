namespace Recipes.Adapters.Web.Dto;

internal sealed record ShoppingListGroupDto(
    string Category,
    IReadOnlyList<ShoppingListItemDto> Items);
