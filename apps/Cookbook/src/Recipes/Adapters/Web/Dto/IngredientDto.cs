namespace Recipes.Adapters.Web.Dto;

internal sealed record IngredientDto(
    Guid Id,
    string Title,
    string Unit,
    float DefaultAmount,
    string Category,
    bool IsSystem);
