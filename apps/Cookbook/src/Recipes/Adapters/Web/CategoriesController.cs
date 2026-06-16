using Microsoft.AspNetCore.Mvc;
using Recipes.Adapters.Web.Dto;
using Recipes.Adapters.Web.Mapping;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/categories")]
internal sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(categories.Select(ToDto).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.CreateAsync(
                request.Name,
                request.Description,
                request.Type.ToDomain(),
                cancellationToken);

            return CreatedAtAction(nameof(GetCategories), new { }, ToDto(category));
        }
        catch (CategoryDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.UpdateAsync(
                CategoryId.From(id),
                request.Name,
                request.Description,
                cancellationToken);

            return NoContent();
        }
        catch (CategoryDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteAsync(CategoryId.From(id), cancellationToken);
            return NoContent();
        }
        catch (CategoryInUseException)
        {
            return BadRequest(ProblemDetailsFor("Category is used in recipes and cannot be deleted."));
        }
        catch (CategoryDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    private static CategoryDto ToDto(Category category) => new(
        category.Id.Value,
        category.Name,
        category.Description,
        category.Type.ToDto());

    private ProblemDetails ProblemDetailsFor(CategoryDomainException ex) =>
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
