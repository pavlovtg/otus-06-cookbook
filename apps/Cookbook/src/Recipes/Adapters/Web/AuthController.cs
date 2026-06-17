using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/auth")]
internal sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Email, request.DisplayName, request.Password, cancellationToken);
            return Ok(ToResponse(result));
        }
        catch (UserEmailAlreadyTakenException)
        {
            return Conflict(ProblemDetailsFor("UserEmailAlreadyTakenException"));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
            return Ok(ToResponse(result));
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(ProblemDetailsFor("InvalidCredentialsException"));
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userInfo = _authService.GetCurrentUser(User);
        if (userInfo is null)
            return Unauthorized();

        return Ok(new UserDto(userInfo.Id, userInfo.Email, userInfo.DisplayName, userInfo.Role));
    }

    private static AuthResponse ToResponse(AuthResult result) =>
        new(result.Token, new UserDto(result.User.Id, result.User.Email, result.User.DisplayName, result.User.Role));

    private ProblemDetails ProblemDetailsFor(string detail) => new()
    {
        Type = "about:blank",
        Status = 400,
        Title = "Validation Error",
        Detail = detail,
        Instance = HttpContext.Request.Path,
    };
}
