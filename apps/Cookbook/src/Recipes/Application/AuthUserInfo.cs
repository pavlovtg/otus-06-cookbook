namespace Recipes.Application;

internal sealed record AuthUserInfo(Guid Id, string Email, string DisplayName, string Role);
