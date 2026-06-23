using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/dashboard")]
[AllowAnonymous]
internal sealed class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly IAuthService _authService;

    public DashboardController(DashboardService dashboardService, IAuthService authService)
    {
        _dashboardService = dashboardService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser(User);
        var userId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;
        var isAdmin = string.Equals(currentUser?.Role, "admin", StringComparison.OrdinalIgnoreCase);

        var view = await _dashboardService.GetStatsAsync(userId, isAdmin, cancellationToken);

        var dto = new DashboardStatsDto(
            TotalRecipes: view.TotalRecipes,
            MyRecipes: view.MyRecipes,
            MyComments: view.MyComments,
            Top10ByRating: view.Top10ByRating.Select(r => new RecipeRankDto(r.Id.Value, r.Title, r.AverageRating)).ToList(),
            TopFavoritesByRating: view.TopFavoritesByRating.Select(r => new RecipeRankDto(r.Id.Value, r.Title, r.AverageRating)).ToList(),
            ByMainIngredient: view.ByMainIngredient.Select(c => new CategoryCountDto(c.CategoryName, c.RecipeCount)).ToList(),
            ByCuisine: view.ByCuisine.Select(c => new CategoryCountDto(c.CategoryName, c.RecipeCount)).ToList(),
            TotalUsers: view.TotalUsers,
            TotalComments: view.TotalComments,
            TopUsersByRating: view.TopUsersByRating?.Select(u => new UserRankDto(u.Id.Value, u.DisplayName, u.AverageRating, u.CommentCount)).ToList(),
            TopUsersByComments: view.TopUsersByComments?.Select(u => new UserRankDto(u.Id.Value, u.DisplayName, u.AverageRating, u.CommentCount)).ToList(),
            PlanFill: view.PlanFill);

        return Ok(dto);
    }
}
