using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/meal-plan")]
[Authorize]
internal sealed class MealPlanController : ControllerBase
{
    private readonly MealPlanService _mealPlanService;
    private readonly IAuthService _authService;

    public MealPlanController(MealPlanService mealPlanService, IAuthService authService)
    {
        _mealPlanService = mealPlanService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMealPlan(CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser(User);
        if (currentUser is null)
            return Unauthorized(UnauthorizedProblemDetails());

        var userId = UserId.From(currentUser.Id);
        var view = await _mealPlanService.GetAsync(userId, cancellationToken);
        return Ok(ToDto(view));
    }

    [HttpPut]
    public async Task<IActionResult> ReplaceMealPlan([FromBody] MealPlanRequest request, CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser(User);
        if (currentUser is null)
            return Unauthorized(UnauthorizedProblemDetails());

        var userId = UserId.From(currentUser.Id);

        try
        {
            var slots = request.Slots
                .Select(s => new MealPlanSlotInput(
                    (WeekDay)s.WeekDay,
                    (MealType)s.MealType,
                    s.Items
                        .Select(i => new MealPlanItemInput(RecipeId.From(i.RecipeId), Servings.From(i.Servings)))
                        .ToList()))
                .ToList();

            var view = await _mealPlanService.ReplaceAsync(userId, slots, cancellationToken);
            return Ok(ToDto(view));
        }
        catch (ServingsOutOfRangeException ex)
        {
            return BadRequest(ProblemDetailsFor(ex.GetType().Name));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearMealPlan(CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser(User);
        if (currentUser is null)
            return Unauthorized(UnauthorizedProblemDetails());

        var userId = UserId.From(currentUser.Id);
        await _mealPlanService.ClearAsync(userId, cancellationToken);
        return NoContent();
    }

    private static MealPlanDto ToDto(MealPlanView view) =>
        new(
            view.Id,
            view.Slots
                .Select(s => new MealPlanSlotDto(
                    (int)s.WeekDay,
                    (int)s.MealType,
                    s.Items
                        .Select(i => new MealPlanItemDto(
                            i.Id,
                            i.RecipeId,
                            i.RecipeTitle,
                            i.Servings))
                        .ToList()))
                .ToList());

    private ProblemDetails ProblemDetailsFor(string detail) => new()
    {
        Type = "about:blank",
        Status = 400,
        Title = "Validation Error",
        Detail = detail,
        Instance = HttpContext.Request.Path,
    };

    private ProblemDetails UnauthorizedProblemDetails() => new()
    {
        Type = "about:blank",
        Status = 401,
        Title = "Unauthorized",
        Detail = "Authentication is required to perform this action.",
        Instance = HttpContext.Request.Path,
    };
}
