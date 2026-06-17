namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeShortDto(
    Guid Id,
    string Title,
    string Description,
    int CookingTime,
    string Difficulty,
    Guid? PhotoId,
    IReadOnlyList<Guid> CategoryIds,
    bool IsPublic,
    string? AuthorName,
    Guid? AuthorId,
    bool? IsFavorite,
    float? AverageRating,
    int? MyRating);
