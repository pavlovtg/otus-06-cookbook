using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Adapters.Web.Mapping;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/ingredients")]
internal sealed class IngredientsController : ControllerBase
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 100;
    private const int MaxPageSize = 1000;

    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients(
        [FromQuery] string? title,
        [FromQuery] string? category,
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return BadRequest(ProblemDetailsFor("'page' must be greater than or equal to 1."));

        if (pageSize < 1)
            return BadRequest(ProblemDetailsFor("'pageSize' must be greater than or equal to 1."));

        if (title is not null && title.Length > IngredientConstraints.TitleMaxLength)
            return BadRequest(ProblemDetailsFor($"'title' must not exceed {IngredientConstraints.TitleMaxLength} characters."));

        pageSize = Math.Min(pageSize, MaxPageSize);

        IngredientCategory? categoryFilter = null;

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!IngredientCategoryDtoExtensions.TryParseCategory(category, out var parsed))
                return BadRequest(ProblemDetailsFor($"Invalid category value: '{category}'."));

            categoryFilter = parsed.ToDomain();
        }

        var result = await _ingredientService.GetIngredientsAsync(
            page, pageSize, title, categoryFilter, cancellationToken);

        var dto = new PagedResult<IngredientDto>(
            result.Items.Select(ToDto).ToList(),
            result.Total,
            result.Page,
            result.PageSize);

        return Ok(dto);
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
            if (!IngredientCategoryDtoExtensions.TryParseCategory(request.Category, out var categoryDto))
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
            if (!IngredientCategoryDtoExtensions.TryParseCategory(request.Category, out var categoryDto))
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
