using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class RecipePhotoService : IRecipePhotoService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipePhotoRepository _photoRepository;
    private readonly ImageSharpThumbnailGenerator _thumbnailGenerator;

    public RecipePhotoService(
        IRecipeRepository recipeRepository,
        IRecipePhotoRepository photoRepository,
        ImageSharpThumbnailGenerator thumbnailGenerator)
    {
        _recipeRepository = recipeRepository;
        _photoRepository = photoRepository;
        _thumbnailGenerator = thumbnailGenerator;
    }

    public async Task<RecipePhotoId> UploadAsync(
        RecipeId recipeId,
        string mimeType,
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        // Fail-fast: проверяем MIME до материализации потока
        RecipePhotoValidator.ValidateMime(mimeType);

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken)
            ?? throw new RecipeNotFoundException(recipeId);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        var originalData = ms.ToArray();

        // Проверяем размер после материализации
        RecipePhotoValidator.ValidateSize(originalData.LongLength);

        var thumbnailData = _thumbnailGenerator.Generate(originalData);

        // Если у рецепта уже есть фото — удалить старое
        if (recipe.PhotoId.HasValue)
            await _photoRepository.DeleteAsync(recipe.PhotoId.Value, cancellationToken);

        var photo = RecipePhoto.Create(RecipePhotoId.New(), recipeId, mimeType, originalData, thumbnailData);
        await _photoRepository.SaveAsync(photo, cancellationToken);

        recipe.SetPhoto(photo.Id);
        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        await _recipeRepository.CommitAsync(cancellationToken);

        return photo.Id;
    }

    public async Task DeleteAsync(RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken)
            ?? throw new RecipeNotFoundException(recipeId);

        if (!recipe.PhotoId.HasValue)
            return;

        await _photoRepository.DeleteAsync(recipe.PhotoId.Value, cancellationToken);
        recipe.ClearPhoto();
        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        await _recipeRepository.CommitAsync(cancellationToken);
    }

    public async Task<byte[]> GetOriginalAsync(RecipePhotoId id, CancellationToken cancellationToken = default)
    {
        var data = await _photoRepository.GetOriginalAsync(id, cancellationToken);
        if (data is null)
            throw new RecipePhotoNotFoundException(id);
        return data;
    }

    public async Task<byte[]> GetThumbnailAsync(RecipePhotoId id, CancellationToken cancellationToken = default)
    {
        var data = await _photoRepository.GetThumbnailAsync(id, cancellationToken);
        if (data is null)
            throw new RecipePhotoNotFoundException(id);
        return data;
    }
}
