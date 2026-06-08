using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application.Ports;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/recipes/v1")]
internal sealed class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public IAsyncEnumerable<RecipeDto> GetRecipes(CancellationToken cancellationToken)
        => _recipeService.GetAllAsync(cancellationToken)
            .Select(r => new RecipeDto(r.Id.Value, r.Title, r.Description));
}
