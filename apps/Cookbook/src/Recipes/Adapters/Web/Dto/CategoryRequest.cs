namespace Recipes.Adapters.Web.Dto;

internal sealed record CategoryRequest(
    string Name,
    string Description,
    CategoryTypeDto Type);
