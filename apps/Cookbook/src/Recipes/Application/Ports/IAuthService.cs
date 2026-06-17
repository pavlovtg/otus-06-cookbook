using System.Security.Claims;

namespace Recipes.Application.Ports;

internal interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string displayName, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    AuthUserInfo? GetCurrentUser(ClaimsPrincipal principal);
}
