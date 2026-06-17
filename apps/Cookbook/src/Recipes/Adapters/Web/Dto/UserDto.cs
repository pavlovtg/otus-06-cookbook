namespace Recipes.Adapters.Web.Dto;

internal sealed record UserDto(Guid Id, string Email, string DisplayName, string Role);
