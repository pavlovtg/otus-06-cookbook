using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Adapters.Web.Mapping;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/shopping-list")]
[Authorize]
internal sealed class ShoppingListController : ControllerBase
{
    private readonly ShoppingListService _shoppingListService;
    private readonly IAuthService _authService;

    public ShoppingListController(ShoppingListService shoppingListService, IAuthService authService)
    {
        _shoppingListService = shoppingListService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetShoppingList(CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser(User);
        if (currentUser is null)
            return Unauthorized(UnauthorizedProblemDetails());

        var userId = UserId.From(currentUser.Id);
        var groups = await _shoppingListService.GetAsync(userId, cancellationToken);

        var dto = groups
            .Select(g => new ShoppingListGroupDto(
                g.Category.ToDtoString(),
                g.Items
                    .Select(i => new ShoppingListItemDto(
                        i.IngredientId.Value,
                        i.Title,
                        i.Amount,
                        i.Unit))
                    .ToList()))
            .ToList();

        return Ok(dto);
    }

    private ProblemDetails UnauthorizedProblemDetails() => new()
    {
        Type = "about:blank",
        Status = 401,
        Title = "Unauthorized",
        Detail = "Authentication is required to perform this action.",
        Instance = HttpContext.Request.Path,
    };
}
