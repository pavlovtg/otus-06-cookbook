namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeDto(
    Guid Id,
    string Title,
    string Description,
    int CookingTime,
    string Difficulty,
    int Servings,
    string Instructions,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    Guid? PhotoId,
    IReadOnlyList<Guid> CategoryIds,
    bool IsPublic,
    string? AuthorName,
    Guid? AuthorId,
    float? AverageRating,
    int? MyRating);
