using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Adapters.Web.Mapping;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/ingredients")]
internal sealed class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients(
        [FromQuery] string? title,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        IngredientCategory? categoryFilter = null;

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!Enum.TryParse<IngredientCategoryDto>(category, ignoreCase: true, out var parsed))
                return BadRequest(ProblemDetailsFor($"Invalid category value: '{category}'."));

            categoryFilter = parsed.ToDomain();
        }

        var ingredients = new List<IngredientDto>();
        await foreach (var ingredient in _ingredientService.GetIngredientsAsync(title, categoryFilter, cancellationToken))
            ingredients.Add(ToDto(ingredient));

        return Ok(ingredients);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetIngredient(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ingredient = await _ingredientService.GetByIdAsync(IngredientId.From(id), cancellationToken);
            return Ok(ToDto(ingredient));
        }
        catch (IngredientDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateIngredient([FromBody] IngredientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<IngredientCategoryDto>(request.Category, ignoreCase: true, out var categoryDto))
                return BadRequest(ProblemDetailsFor($"Invalid category value: '{request.Category}'."));

            var ingredient = await _ingredientService.CreateAsync(
                request.Title,
                request.Unit,
                request.DefaultAmount,
                categoryDto.ToDomain(),
                cancellationToken);

            return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id.Value }, ToDto(ingredient));
        }
        catch (IngredientDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] IngredientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<IngredientCategoryDto>(request.Category, ignoreCase: true, out var categoryDto))
                return BadRequest(ProblemDetailsFor($"Invalid category value: '{request.Category}'."));

            await _ingredientService.UpdateAsync(
                IngredientId.From(id),
                request.Title,
                request.Unit,
                request.DefaultAmount,
                categoryDto.ToDomain(),
                cancellationToken);

            return NoContent();
        }
        catch (IngredientDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteIngredient(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _ingredientService.DeleteAsync(IngredientId.From(id), cancellationToken);
            return NoContent();
        }
        catch (IngredientDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    private static IngredientDto ToDto(Ingredient ingredient) => new(
        ingredient.Id.Value,
        ingredient.Title,
        ingredient.Unit,
        ingredient.DefaultAmount,
        ingredient.Category.ToDto(),
        ingredient.IsSystem);

    private ProblemDetails ProblemDetailsFor(IngredientDomainException ex) =>
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
