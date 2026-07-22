using Application.Common;
using Application.Interfaces.Services;
using Domain.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services
{
    public class ImageProcessor : IImageProcessor
    {
        public async Task<FileDto> ProcessAsync(FileDto file, ImageType imageType, CancellationToken cancellationToken = default)
        {
            var inputStream = file.Content;

            // 1. Reset stream position if it was previously read
            if (inputStream.CanSeek && inputStream.Position > 0)
            {
                inputStream.Position = 0;
            }

            // 2. Load image from memory stream
            using var image = await Image.LoadAsync(inputStream, cancellationToken);

            // 3. Determine size target based on domain type
            var targetSize = imageType switch
            {
                ImageType.Category => new Size(800, 800),
                ImageType.Product => new Size(1000, 800),
                ImageType.UserProfile => new Size(200, 200),
                _ => throw new ArgumentOutOfRangeException(nameof(imageType), $"Unsupported image type: {imageType}")
            };

            // 4. Resize safely without stretching/distorting aspect ratio
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = targetSize,
                Mode = ResizeMode.Max
            }));

            // 5. Output memory stream for WebP conversion
            var outputStream = new MemoryStream();

            await image.SaveAsync(outputStream, new WebpEncoder { Quality = 80 }, cancellationToken);

            // 6. Reset position so storage services can read from byte 0
            outputStream.Position = 0;

            // 7. Return updated FileDto
            return new FileDto(outputStream,
                "image/webp",
                Path.ChangeExtension(file.FileName, ".webp");
        }
    }
}