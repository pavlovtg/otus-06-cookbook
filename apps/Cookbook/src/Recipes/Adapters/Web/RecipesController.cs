using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Application;
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
    public IAsyncEnumerable<RecipeShortDto> GetRecipes(CancellationToken cancellationToken)
        => _recipeService.GetRecipesAsync(cancellationToken).Select(ToShortDto);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecipe(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var details = await _recipeService.GetByIdWithDetailsAsync(RecipeId.From(id), cancellationToken);
            return Ok(ToDto(details));
        }
        catch (RecipeDomainException ex)
        {
            return NotFound(ProblemDetailsFor(ex));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var difficulty))
                return BadRequest(ProblemDetailsFor($"Invalid difficulty value: '{request.Difficulty}'."));

            var ingredientInputs = MapIngredientInputs(request.Ingredients);

            var recipe = await _recipeService.CreateAsync(
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                ingredientInputs,
                cancellationToken);

            var details = await _recipeService.GetByIdWithDetailsAsync(recipe.Id, cancellationToken);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id.Value }, ToDto(details));
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

            var ingredientInputs = MapIngredientInputs(request.Ingredients);

            await _recipeService.UpdateAsync(
                RecipeId.From(id),
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                ingredientInputs,
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
            return NotFound(ProblemDetailsFor(ex));
        }
    }

    private static IEnumerable<RecipeIngredientInput> MapIngredientInputs(
        IReadOnlyList<RecipeIngredientRequest> items)
        => items.Select(i => new RecipeIngredientInput(IngredientId.From(i.IngredientId), i.Amount));

    private static RecipeShortDto ToShortDto(Recipe recipe) => new(
        recipe.Id.Value,
        recipe.Title,
        recipe.Description,
        recipe.CookingTime,
        recipe.Difficulty.ToString().ToLowerInvariant());

    private static RecipeDto ToDto(RecipeWithIngredientDetails details) => new(
        details.Recipe.Id.Value,
        details.Recipe.Title,
        details.Recipe.Description,
        details.Recipe.CookingTime,
        details.Recipe.Difficulty.ToString().ToLowerInvariant(),
        details.Recipe.Servings,
        details.Recipe.Instructions,
        details.Ingredients
            .Select(i => new RecipeIngredientDto(i.IngredientId.Value, i.Title, i.Amount, i.Unit))
            .ToList());

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
