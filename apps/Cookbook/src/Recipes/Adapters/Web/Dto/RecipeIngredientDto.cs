namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeIngredientDto(
    Guid IngredientId,
    string Title,
    decimal Amount,
    string Unit);
