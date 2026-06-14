using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipePhotoTests
{
    private static readonly RecipePhotoId ValidPhotoId = RecipePhotoId.New();
    private static readonly RecipeId ValidRecipeId = RecipeId.New();
    private static readonly byte[] ValidData = new byte[1024];

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    public void Create_WithValidMime_ReturnsPhoto(string mimeType)
    {
        var photo = RecipePhoto.Create(ValidPhotoId, ValidRecipeId, mimeType, ValidData, ValidData);

        Assert.Equal(ValidPhotoId, photo.Id);
        Assert.Equal(ValidRecipeId, photo.RecipeId);
        Assert.Equal(ValidData, photo.OriginalData);
        Assert.Equal(ValidData, photo.ThumbnailData);
    }

    [Theory]
    [InlineData("image/gif")]
    [InlineData("image/webp")]
    [InlineData("application/octet-stream")]
    [InlineData("text/plain")]
    public void Create_WithInvalidMime_ThrowsException(string mimeType)
    {
        Assert.Throws<RecipePhotoInvalidMimeException>(() =>
            RecipePhoto.Create(ValidPhotoId, ValidRecipeId, mimeType, ValidData, ValidData));
    }

    [Fact]
    public void Create_WithDataExceedingMaxSize_ThrowsException()
    {
        var oversizedData = new byte[11 * 1024 * 1024]; // 11 МБ

        Assert.Throws<RecipePhotoTooLargeException>(() =>
            RecipePhoto.Create(ValidPhotoId, ValidRecipeId, "image/jpeg", oversizedData, ValidData));
    }

    [Fact]
    public void Create_WithDataAtMaxSize_ReturnsPhoto()
    {
        var maxData = new byte[10 * 1024 * 1024]; // ровно 10 МБ

        var photo = RecipePhoto.Create(ValidPhotoId, ValidRecipeId, "image/jpeg", maxData, ValidData);

        Assert.NotNull(photo);
    }
}
