using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/recipes")]
internal sealed class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public IAsyncEnumerable<RecipeDto> GetRecipes(CancellationToken cancellationToken)
        => _recipeService.GetRecipesAsync(cancellationToken).Select(ToDto);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecipe(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipeService.GetByIdAsync(RecipeId.From(id), cancellationToken);
            return Ok(ToDto(recipe));
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var difficulty))
                return BadRequest(ProblemDetailsFor($"Invalid difficulty value: '{request.Difficulty}'."));

            var recipe = await _recipeService.CreateAsync(
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                cancellationToken);

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id.Value }, ToDto(recipe));
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var difficulty))
                return BadRequest(ProblemDetailsFor($"Invalid difficulty value: '{request.Difficulty}'."));

            await _recipeService.UpdateAsync(
                RecipeId.From(id),
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                cancellationToken);

            return NoContent();
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _recipeService.DeleteAsync(RecipeId.From(id), cancellationToken);
            return NoContent();
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    private static RecipeDto ToDto(Recipe recipe) => new(
        recipe.Id.Value,
        recipe.Title,
        recipe.Description,
        recipe.CookingTime,
        recipe.Difficulty.ToString().ToLowerInvariant(),
        recipe.Servings,
        recipe.Instructions);

    private ProblemDetails ProblemDetailsFor(RecipeDomainException ex) =>
        ProblemDetailsFor(ex.GetType().Name);

    private ProblemDetails ProblemDetailsFor(string detail) => new()
    {
        Type = "about:blank",
        Status = 400,
        Title = "Validation Error",
        Detail = detail,
        Instance = HttpContext.Request.Path,
    };
}
