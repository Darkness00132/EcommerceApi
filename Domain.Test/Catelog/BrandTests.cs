using Domain.Entities.Catalog;
using Domain.Exceptions;

namespace Domain.Test.Catalog;

public class BrandTests
{
    private static Brand CreateBrand(string nameEn = "Nike", string nameAr = "نايكي")
        => new(nameEn, nameAr);

    [Fact]
    public void Constructor_WithValidNames_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var brand = CreateBrand("  Nike  ", "  نايكي  ");

        // Assert
        Assert.NotEqual(Guid.Empty, brand.Id);
        Assert.Equal("Nike", brand.NameEn);
        Assert.Equal("نايكي", brand.NameAr);
        Assert.Empty(brand.Products);
    }

    [Theory]
    [InlineData(null, "نايكي")]
    [InlineData("", "نايكي")]
    [InlineData("   ", "نايكي")]
    [InlineData("Nike", null)]
    [InlineData("Nike", "")]
    [InlineData("Nike", "   ")]
    public void Constructor_WithInvalidNames_ShouldThrowDomainException(string? nameEn, string? nameAr)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Brand(nameEn!, nameAr!));
    }

    [Fact]
    public void UpdateEnglishName_WithValidName_ShouldUpdateAndTrim()
    {
        // Arrange
        var brand = CreateBrand();

        // Act
        brand.UpdateEnglishName("  Adidas  ");

        // Assert
        Assert.Equal("Adidas", brand.NameEn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateEnglishName_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var brand = CreateBrand();

        // Act & Assert
        Assert.Throws<DomainException>(() => brand.UpdateEnglishName(invalidName!));
    }

    [Fact]
    public void UpdateArabicName_WithValidName_ShouldUpdateAndTrim()
    {
        // Arrange
        var brand = CreateBrand();

        // Act
        brand.UpdateArabicName("  أديداس  ");

        // Assert
        Assert.Equal("أديداس", brand.NameAr);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateArabicName_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var brand = CreateBrand();

        // Act & Assert
        Assert.Throws<DomainException>(() => brand.UpdateArabicName(invalidName!));
    }
}
