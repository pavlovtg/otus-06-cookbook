namespace Recipes.Adapters.Web.Dto;

internal sealed record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    CategoryTypeDto Type);
