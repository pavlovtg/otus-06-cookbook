using Microsoft.AspNetCore.Mvc;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Adapters.Web;

[ApiController]
[Route("api/v1/photos")]
internal sealed class PhotosController : ControllerBase
{
    private readonly IRecipePhotoService _photoService;

    public PhotosController(IRecipePhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpGet("{photoId:guid}")]
    public async Task<IActionResult> GetPhoto(Guid photoId, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _photoService.GetOriginalAsync(RecipePhotoId.From(photoId), cancellationToken);
            Response.Headers.CacheControl = "public, max-age=86400";
            return File(data, "image/jpeg");
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

    [HttpGet("{photoId:guid}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(Guid photoId, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _photoService.GetThumbnailAsync(RecipePhotoId.From(photoId), cancellationToken);
            Response.Headers.CacheControl = "public, max-age=86400";
            return File(data, "image/jpeg");
        }
        catch (RecipeDomainException ex)
        {
            return BadRequest(ProblemDetailsFor(ex));
        }
    }

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
