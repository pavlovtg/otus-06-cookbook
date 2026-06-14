using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Recipes.Application;

internal sealed class ImageSharpThumbnailGenerator
{
    private const int ThumbnailWidth = 400;
    private const int ThumbnailHeight = 300;

    public byte[] Generate(byte[] originalData)
    {
        using var image = Image.Load(originalData);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(ThumbnailWidth, ThumbnailHeight),
            Mode = ResizeMode.Crop,
        }));

        using var output = new MemoryStream();
        image.SaveAsJpeg(output);
        return output.ToArray();
    }

    public byte[] ConvertToJpeg(byte[] originalData)
    {
        using var image = Image.Load(originalData);
        using var output = new MemoryStream();
        image.SaveAsJpeg(output);
        return output.ToArray();
    }
}
