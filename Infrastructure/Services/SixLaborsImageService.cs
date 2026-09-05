using Application.Abstractions.Services;
using Application.Common.Files;
using Application.Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services;

internal class SixLaborsImageService : IImageManipulationService
{
    public async Task<FileDto> ResizeImageAsync(
        FileDto file,
        ImageType type,
        CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(file.Content, cancellationToken);

        var size = type switch {
            ImageType.Product => new Size(800, 800),
            ImageType.Category => new Size(500, 500),
            ImageType.Thumbnail => new Size(300, 300),
            _ => new Size(800, 800)
        };

        image.Mutate(x => x.Resize(new ResizeOptions {
            Size = size,
            Mode = ResizeMode.Crop
        }));

        var output = new MemoryStream();

        await image.SaveAsWebpAsync(
            output,
            new WebpEncoder {
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
