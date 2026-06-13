namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeShortDto(
    Guid Id,
    string Title,
    string Description,
    int CookingTime,
    string Difficulty);
