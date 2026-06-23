namespace Recipes.Adapters.Web.Dto;

internal sealed record IngredientRequest(
    string Title,
    string Unit,
    float DefaultAmount,
    string Category,
    bool? IsSystem = null);
