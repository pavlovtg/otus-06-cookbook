using Microsoft.AspNetCore.Authorization;
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
    private readonly IAuthService _authService;

    public RecipesController(IRecipeService recipeService, IRecipePhotoService photoService, IAuthService authService)
    {
        _recipeService = recipeService;
        _photoService = photoService;
        _authService = authService;
    }

    private const int DefaultPage = 1;
    private const int DefaultPageSize = 18;
    private const int MaxPageSize = 1000;

    [HttpGet]
    public async Task<IActionResult> GetRecipes(
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize,
        [FromQuery] string? q = null,
        [FromQuery] string? sort = null,
        [FromQuery] bool? favorites = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return BadRequest(ProblemDetailsFor("'page' must be greater than or equal to 1."));

        if (pageSize < 1)
            return BadRequest(ProblemDetailsFor("'pageSize' must be greater than or equal to 1."));

        pageSize = Math.Min(pageSize, MaxPageSize);

        if (q is not null && q.Length > 300)
            return BadRequest(ProblemDetailsFor("'q' must not exceed 300 characters."));

        var sortOrder = sort switch
        {
            "title_desc" => RecipeSortOrder.TitleDesc,
            _ => RecipeSortOrder.TitleAsc,
        };

        var currentUser = _authService.GetCurrentUser(User);
        var currentUserId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;

        if (favorites == true && currentUserId is null)
            return Unauthorized(UnauthorizedProblemDetails());

        var result = await _recipeService.GetRecipesPagedAsync(page, pageSize, q, sortOrder, currentUserId, favorites, cancellationToken);

        var dto = new PagedResult<RecipeShortDto>(
            result.Items.Select(ToShortDto).ToList(),
            result.Total,
            result.Page,
            result.PageSize);

        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecipe(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser(User);
            var currentUserId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;
            var details = await _recipeService.GetByIdWithDetailsAsync(RecipeId.From(id), currentUserId, cancellationToken);
            return Ok(ToDto(details));
        }
        catch (RecipeForbiddenException)
        {
            return StatusCode(403, ForbiddenProblemDetails());
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var difficulty))
                return BadRequest(ProblemDetailsFor($"Invalid difficulty value: '{request.Difficulty}'."));

            var ingredientInputs = MapIngredientInputs(request.Ingredients);
            var categoryIds = MapCategoryIds(request.CategoryIds);
            var currentUser = _authService.GetCurrentUser(User);
            var authorId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;

            var recipe = await _recipeService.CreateAsync(
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                request.IsPublic,
                authorId,
                ingredientInputs,
                categoryIds,
                cancellationToken);

            var details = await _recipeService.GetByIdWithDetailsAsync(recipe.Id, authorId, cancellationToken);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id.Value }, ToDto(details));
        }
        catch (RecipeUnauthorizedException)
        {
            return Unauthorized(UnauthorizedProblemDetails());
        }
        catch (CategoryDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex.GetType().Name));
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var difficulty))
                return BadRequest(ProblemDetailsFor($"Invalid difficulty value: '{request.Difficulty}'."));

            var ingredientInputs = MapIngredientInputs(request.Ingredients);
            var categoryIds = MapCategoryIds(request.CategoryIds);
            var currentUser = _authService.GetCurrentUser(User);
            var currentUserId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;
            var currentUserRole = ParseRole(currentUser?.Role);

            await _recipeService.UpdateAsync(
                RecipeId.From(id),
                request.Title,
                request.Description,
                request.CookingTime,
                difficulty,
                request.Servings,
                request.Instructions,
                request.IsPublic,
                currentUserId,
                currentUserRole,
                ingredientInputs,
                categoryIds,
                cancellationToken);

            return NoContent();
        }
        catch (RecipeForbiddenException)
        {
            return StatusCode(403, ForbiddenProblemDetails());
        }
        catch (CategoryDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex.GetType().Name));
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser(User);
            var currentUserId = currentUser is not null ? UserId.From(currentUser.Id) : (UserId?)null;
            var currentUserRole = ParseRole(currentUser?.Role);
            await _recipeService.DeleteAsync(RecipeId.From(id), currentUserId, currentUserRole, cancellationToken);
            return NoContent();
        }
        catch (RecipeForbiddenException)
        {
            return StatusCode(403, ForbiddenProblemDetails());
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/favorites")]
    public async Task<IActionResult> AddFavorite(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser(User);
            if (currentUser is null)
                return Unauthorized(UnauthorizedProblemDetails());

            await _recipeService.AddFavoriteAsync(UserId.From(currentUser.Id), RecipeId.From(id), cancellationToken);
            return StatusCode(201);
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}/favorites")]
    public async Task<IActionResult> RemoveFavorite(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser(User);
            if (currentUser is null)
                return Unauthorized(UnauthorizedProblemDetails());

            await _recipeService.RemoveFavoriteAsync(UserId.From(currentUser.Id), RecipeId.From(id), cancellationToken);
            return NoContent();
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [Authorize]
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

    [Authorize]
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

    private static UserRole? ParseRole(string? role) =>
        role is not null && Enum.TryParse<UserRole>(role, ignoreCase: true, out var userRole)
            ? userRole
            : null;

    private static IEnumerable<RecipeIngredientInput> MapIngredientInputs(
        IReadOnlyList<RecipeIngredientRequest> items)
        => items.Select(i => new RecipeIngredientInput(IngredientId.From(i.IngredientId), i.Amount));

    private static IEnumerable<CategoryId> MapCategoryIds(IReadOnlyList<Guid> ids)
        => ids.Select(CategoryId.From);

    private static RecipeShortDto ToShortDto(RecipeShortWithAuthor item) => new(
        item.Recipe.Id.Value,
        item.Recipe.Title,
        item.Recipe.Description,
        item.Recipe.CookingTime,
        item.Recipe.Difficulty.ToString().ToLowerInvariant(),
        item.Recipe.PhotoId?.Value,
        item.Recipe.Categories.Select(c => c.CategoryId.Value).ToList(),
        item.Recipe.IsPublic,
        item.AuthorName,
        item.Recipe.AuthorId?.Value,
        item.IsFavorite);

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
        details.Recipe.Categories.Select(c => c.CategoryId.Value).ToList(),
        details.Recipe.IsPublic,
        details.AuthorName,
        details.Recipe.AuthorId?.Value);

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

    private ProblemDetails ForbiddenProblemDetails() => new()
    {
        Type = "about:blank",
        Status = 403,
        Title = "Forbidden",
        Detail = "You do not have permission to access this resource.",
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
