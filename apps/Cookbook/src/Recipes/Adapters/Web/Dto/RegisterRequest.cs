namespace Recipes.Adapters.Web.Dto;

internal sealed record RegisterRequest(string Email, string DisplayName, string Password);
