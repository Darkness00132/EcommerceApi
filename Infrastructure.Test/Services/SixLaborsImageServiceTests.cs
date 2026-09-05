using Application.Common.Files;
using Application.Constants;
using FluentAssertions;
using Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Infrastructure.Test.Services;

public class SixLaborsImageServiceTests
{
    private readonly SixLaborsImageService _sut;
    private readonly FileDto _file;

    public SixLaborsImageServiceTests()
    {
        _sut = new SixLaborsImageService();
        _file = CreateImage(1600, 1000);
    }

    [Theory]
    [InlineData(ImageType.Product, 800, 800)]
    [InlineData(ImageType.Category, 500, 500)]
    [InlineData(ImageType.Thumbnail, 300, 300)]
    public async Task Resize_Image_According_To_Its_Purpose(
        ImageType type,
        int expectedWidth,
        int expectedHeight)
    {
        // Act
        var result = await _sut.ResizeImageAsync(_file, type);

        // Assert
        result.FileName.Should().Be("test.webp");
        result.ContentType.Should().Be("image/webp");

        using var image = await Image.LoadAsync(result.Content);

        image.Width.Should().Be(expectedWidth);
        image.Height.Should().Be(expectedHeight);
    }

    private static FileDto CreateImage(int width, int height)
    {
        var stream = new MemoryStream();

        using (var image = new Image<Rgba32>(width, height)) {
            image.SaveAsPng(stream);
        }

        stream.Position = 0;

        return new FileDto(
            "test.jpg",
            "image/jpeg",
            stream.Length,
            stream);
    }
}
