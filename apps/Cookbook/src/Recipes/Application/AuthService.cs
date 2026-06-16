using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class AuthService : IAuthService
{
    private const int TokenTtlHours = 24;

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtSecret = configuration["JWT:Secret"]
            ?? throw new InvalidOperationException("JWT:Secret is not configured.");
        _jwtIssuer = configuration["JWT:Issuer"] ?? "cookbook";
        _jwtAudience = configuration["JWT:Audience"] ?? "cookbook";
    }

    public async Task<AuthResult> RegisterAsync(
        string email,
        string displayName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new UserEmailAlreadyTakenException(email);

        var passwordHash = _passwordHasher.Hash(password);
        var user = User.Create(UserId.New(), email, displayName, passwordHash, UserRole.User);

        await _userRepository.CreateAsync(user, cancellationToken);
        await _userRepository.CommitAsync(cancellationToken);

        var token = GenerateToken(user);
        return new AuthResult(token, ToUserInfo(user));
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        if (user is null)
            throw new InvalidCredentialsException();

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = GenerateToken(user);
        return new AuthResult(token, ToUserInfo(user));
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        // JWT stateless — сессия хранится на стороне BFF
        return Task.CompletedTask;
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.ToString().ToLowerInvariant()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(TokenTtlHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AuthUserInfo? GetCurrentUser(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email);
        var displayName = principal.FindFirstValue(JwtRegisteredClaimNames.Name) ?? string.Empty;
        var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        if (sub is null || email is null || !Guid.TryParse(sub, out var id))
            return null;

        return new AuthUserInfo(id, email, displayName, role);
    }

    private static AuthUserInfo ToUserInfo(User user) =>
        new(user.Id.Value, user.Email, user.DisplayName, user.Role.ToString().ToLowerInvariant());
}
