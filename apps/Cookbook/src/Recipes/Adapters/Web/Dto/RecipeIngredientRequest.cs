namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeIngredientRequest(
    Guid IngredientId,
    decimal Amount);
