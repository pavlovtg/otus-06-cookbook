namespace Recipes.Adapters.Web.Dto;

internal sealed record IngredientDto(
    Guid Id,
    string Title,
    string Unit,
    float DefaultAmount,
    IngredientCategoryDto Category,
    bool IsSystem);
