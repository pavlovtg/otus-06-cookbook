using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipePhotoRepository
{
    Task SaveAsync(RecipePhoto photo, CancellationToken cancellationToken = default);
    Task<byte[]?> GetOriginalAsync(RecipePhotoId id, CancellationToken cancellationToken = default);
    Task<byte[]?> GetThumbnailAsync(RecipePhotoId id, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipePhotoId id, CancellationToken cancellationToken = default);
}
