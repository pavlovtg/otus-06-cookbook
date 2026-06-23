namespace Recipes.Adapters.Web.Dto;

internal sealed record CategoryCountDto(
    string CategoryName,
    int RecipeCount);
