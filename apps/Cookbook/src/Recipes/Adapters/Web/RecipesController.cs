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
    private readonly IRecipePhotoService _photoService;

    public RecipesController(IRecipeService recipeService, IRecipePhotoService photoService)
    {
        _recipeService = recipeService;
        _photoService = photoService;
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

            var ingredientInputs = MapIngredientInputs(request.Ingredients);
            var categoryIds = MapCategoryIds(request.CategoryIds);

            var recipe = await _recipeService.CreateAsync(
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                ingredientInputs,
                categoryIds,
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
            var categoryIds = MapCategoryIds(request.CategoryIds);

            await _recipeService.UpdateAsync(
                RecipeId.From(id),
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                ingredientInputs,
                categoryIds,
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

    [HttpPost("{id:guid}/photo")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var photoId = await _photoService.UploadAsync(
                RecipeId.From(id),
                file.ContentType,
                file.OpenReadStream(),
                cancellationToken);
            return Ok(new { photoId = photoId.Value });
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpDelete("{id:guid}/photo")]
    public async Task<IActionResult> DeletePhoto(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _photoService.DeleteAsync(RecipeId.From(id), cancellationToken);
            return NoContent();
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    private static IEnumerable<RecipeIngredientInput> MapIngredientInputs(
        IReadOnlyList<RecipeIngredientRequest> items)
        => items.Select(i => new RecipeIngredientInput(IngredientId.From(i.IngredientId), i.Amount));

    private static IEnumerable<CategoryId> MapCategoryIds(IReadOnlyList<Guid> ids)
        => ids.Select(CategoryId.From);

    private static RecipeShortDto ToShortDto(Recipe recipe) => new(
        recipe.Id.Value,
        recipe.Title,
        recipe.Description,
        recipe.CookingTime,
        recipe.Difficulty.ToString().ToLowerInvariant(),
        recipe.PhotoId?.Value,
        recipe.Categories.Select(c => c.CategoryId.Value).ToList());

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
            .ToList(),
        details.Recipe.PhotoId?.Value,
        details.Recipe.Categories.Select(c => c.CategoryId.Value).ToList());

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
