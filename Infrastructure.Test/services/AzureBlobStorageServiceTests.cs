using System.Text;
using Application.Common.Files;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Services.Storage;
using Moq;

namespace Infrastructure.Test.services;

public class AzureBlobStorageServiceTests
{
    private readonly Mock<BlobContainerClient> _containerClientMock;
    private readonly AzureBlobStorageService _sut;

    public AzureBlobStorageServiceTests()
    {
        _containerClientMock = new Mock<BlobContainerClient>();
        _sut = new AzureBlobStorageService(_containerClientMock.Object);
    }

    [Fact]
    public async Task UploadAsync_ShouldUploadFileAndReturnBlobKey_WhenDestinationIsProvided()
    {
        // Arrange
        var file = CreateTestFile("test.png", "image/png", "sample content");
        var destination = "products";

        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(c => c.GetBlobClient(It.Is<string>(k => k.StartsWith("products/"))))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<BlobUploadOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Act
        var result = await _sut.UploadAsync(file, destination);

        // Assert
        Assert.StartsWith("products/", result);
        Assert.EndsWith("_test.png", result);

        blobClientMock.Verify(
            b => b.UploadAsync(
                It.IsAny<Stream>(),
                It.Is<BlobUploadOptions>(o => o.HttpHeaders.ContentType == "image/png"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadAsync_ShouldReturnBlobKeyWithoutLeadingSlash_WhenDestinationIsEmpty()
    {
        // Arrange
        var file = CreateTestFile("avatar.jpg", "image/jpeg", "avatar content");
        var destination = string.Empty;

        var blobClientMock = new Mock<BlobClient>();

        // Set up GetBlobClient to accept any string key
        _containerClientMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<BlobUploadOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Act
        var result = await _sut.UploadAsync(file, destination);

        // Assert
        Assert.False(result.StartsWith("/"));
        Assert.EndsWith("_avatar.jpg", result);

        // Verify GetBlobClient was called with a key that does not start with '/'
        _containerClientMock.Verify(
            c => c.GetBlobClient(It.Is<string>(k => !k.StartsWith("/"))),
            Times.Once);
    }

    [Fact]
    public async Task UploadAsync_ShouldResetStreamPositionToZero_WhenStreamIsSeekableAndNotAtOrigin()
    {
        // Arrange
        var file = CreateTestFile("doc.pdf", "application/pdf", "pdf content");
        file.Content.Position = 5;

        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<BlobUploadOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Act
        await _sut.UploadAsync(file, "docs");

        // Assert
        Assert.Equal(0, file.Content.Position);
    }

    [Fact]
    public async Task UploadManyAsync_ShouldUploadAllFiles_AndReturnAllKeys()
    {
        // Arrange
        var files = new List<FileDto>
        {
            CreateTestFile("img1.png", "image/png", "content1"),
            CreateTestFile("img2.png", "image/png", "content2")
        };

        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<BlobUploadOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Act
        var results = (await _sut.UploadManyAsync(files, "gallery")).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.EndsWith("_img1.png", results[0]);
        Assert.EndsWith("_img2.png", results[1]);
    }

    [Fact]
    public async Task DeleteAsync_ShouldTrimLeadingSlash_AndDeleteBlobIfExists()
    {
        // Arrange
        var keyWithLeadingSlash = "/products/shirt.png";
        var expectedCleanKey = "products/shirt.png";

        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(c => c.GetBlobClient(expectedCleanKey))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        // Act
        await _sut.DeleteAsync(keyWithLeadingSlash);

        // Assert
        _containerClientMock.Verify(c => c.GetBlobClient(expectedCleanKey), Times.Once);
        blobClientMock.Verify(
            b => b.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteAsync_ShouldDoNothing_WhenKeyIsNullOrWhitespace(string? invalidKey)
    {
        // Act
        await _sut.DeleteAsync(invalidKey!);

        // Assert
        _containerClientMock.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteManyAsync_ShouldDeleteOnlyValidKeys()
    {
        // Arrange
        var keys = new List<string> { "items/1.png", "", "/items/2.png", null! };

        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(b => b.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        // Act
        await _sut.DeleteManyAsync(keys);

        // Assert
        _containerClientMock.Verify(c => c.GetBlobClient("items/1.png"), Times.Once);
        _containerClientMock.Verify(c => c.GetBlobClient("items/2.png"), Times.Once);
        _containerClientMock.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Exactly(2));
    }

    private static FileDto CreateTestFile(string fileName, string contentType, string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new FileDto(fileName, contentType, 1024 * 1024 * 5, stream);
    }
}
