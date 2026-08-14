using Application.Abstractions.Services;
using Application.Common.Files;
using Application.Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services;

internal sealed class SixLaborsImageService : IImageManipulationService
{
    public async Task<FileDto> ResizeImage(
        FileDto file,
        ImageType type,
        CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(file.Content, cancellationToken);

        var width = type switch
        {
            ImageType.Product => 800,
            ImageType.Category => 500,
            ImageType.Thumbnail => 300,
            _ => 800
        };

        if (image.Width > width)
        {
            image.Mutate(x => x.Resize(width, 0));
        }

        var output = new MemoryStream();

        await image.SaveAsWebpAsync(
            output,
            new WebpEncoder
            {
                Quality = 80
            },
            cancellationToken);

        output.Position = 0;

        return new FileDto(
            $"{Path.GetFileNameWithoutExtension(file.FileName)}.webp",
            "image/webp",
            output.Length,
            output);
    }
}