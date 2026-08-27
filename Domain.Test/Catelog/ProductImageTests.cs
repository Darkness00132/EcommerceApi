using Domain.Entities.Catalog;
using Domain.Exceptions;

namespace Domain.Test.Catalog;

public class ProductImageTests
{
    private static readonly Guid DefaultProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var image = new ProductImage(DefaultProductId, "  products/item-1.jpg  ");

        // Assert
        Assert.Equal(DefaultProductId, image.ProductId);
        Assert.Equal("products/item-1.jpg", image.ImageKey);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new ProductImage(Guid.Empty, "products/item-1.jpg"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidImageKey_ShouldThrowDomainException(string? invalidKey)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new ProductImage(DefaultProductId, invalidKey!));
    }
}
