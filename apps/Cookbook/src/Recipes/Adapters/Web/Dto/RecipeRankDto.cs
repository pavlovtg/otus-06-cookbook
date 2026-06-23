namespace Recipes.Adapters.Web.Dto;

internal sealed record RecipeRankDto(
    Guid Id,
    string Title,
    float AverageRating);
