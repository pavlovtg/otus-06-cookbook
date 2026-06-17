namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeRequest(
    string Title,
    string? Description,
    int CookingTime,
    string Difficulty,
    int Servings,
    string Instructions,
    bool IsPublic,
    IReadOnlyList<RecipeIngredientRequest> Ingredients,
    IReadOnlyList<Guid> CategoryIds);
