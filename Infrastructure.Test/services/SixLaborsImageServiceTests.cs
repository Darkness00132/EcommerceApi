using Application.Common.Files;
using Application.Constants;
using Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Infrastructure.Test.services;

public class SixLaborsImageServiceTests
{
    private readonly SixLaborsImageService _service = new();

    [Theory]
    [InlineData(ImageType.Product, 1000, 800)]
    [InlineData(ImageType.Category, 800, 500)]
    [InlineData(ImageType.Thumbnail, 500, 300)]
    public async Task ResizeImageAsync_ShouldResize_WhenImageIsWiderThanThreshold(
        ImageType type,
        int originalWidth,
        int expectedWidth)
    {
        // Arrange
        var inputMemoryStream = CreateTestImageStream(originalWidth, 600);
        var inputDto = new FileDto("sample-photo.png", "image/png", inputMemoryStream.Length, inputMemoryStream);

        // Act
        var result = await _service.ResizeImageAsync(inputDto, type);

        // Assert
        Assert.Equal("sample-photo.webp", result.FileName);
        Assert.Equal("image/webp", result.ContentType);
        Assert.True(result.Content.Length > 0);
        Assert.Equal(0, result.Content.Position); // Stream must be rewound

        // Verify output dimensions using ImageSharp
        using var resizedImage = await Image.LoadAsync(result.Content);
        Assert.Equal(expectedWidth, resizedImage.Width);
    }

    [Fact]
    public async Task ResizeImageAsync_ShouldNotResize_WhenImageIsSmallerThanThreshold()
    {
        // Arrange: Image width (200px) is smaller than Thumbnail target (300px)
        const int smallWidth = 200;
        var inputMemoryStream = CreateTestImageStream(smallWidth, 200);
        var inputDto = new FileDto("small-icon.jpg", "image/jpeg", inputMemoryStream.Length, inputMemoryStream);

        // Act
        var result = await _service.ResizeImageAsync(inputDto, ImageType.Thumbnail);

        // Assert
        using var outputImage = await Image.LoadAsync(result.Content);
        Assert.Equal(smallWidth, outputImage.Width);
    }

    /// <summary>
    /// Helper method to create an in-memory test image stream dynamically without disk I/O.
    /// </summary>
    private static MemoryStream CreateTestImageStream(int width, int height)
    {
        using var image = new Image<Rgba32>(width, height);
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        stream.Position = 0; // Reset position so reader can read from start
        return stream;
    }
}
