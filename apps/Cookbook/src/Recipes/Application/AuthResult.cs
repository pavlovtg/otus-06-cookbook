namespace Recipes.Application;

internal sealed record AuthResult(string Token, AuthUserInfo User);
