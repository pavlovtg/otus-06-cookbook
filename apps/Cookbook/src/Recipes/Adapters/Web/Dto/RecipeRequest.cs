namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeRequest(
    string Title,
    string? Description,
    int CookingTime,
    string Difficulty,
    int Servings,
    string Instructions,
    IReadOnlyList<RecipeIngredientRequest> Ingredients,
    IReadOnlyList<Guid> CategoryIds);
