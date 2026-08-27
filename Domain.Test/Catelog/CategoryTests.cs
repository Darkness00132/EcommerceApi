using Domain.Entities.Catalog;
using Domain.Exceptions;

namespace Domain.Test.Catalog;

public class CategoryTests
{
    // Centralized factory method to provide valid default values
    private static Category CreateCategory(
        string nameEn = "Electronics",
        string nameAr = "إلكترونيات",
        string imageKey = "categories/electronics.jpg",
        string? descriptionEn = "Electronic devices and accessories",
        string? descriptionAr = "الأجهزة الإلكترونية والملحقات")
        => new(nameEn, nameAr, imageKey, descriptionEn, descriptionAr);

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesAndTrim()
    {
        // Act
        var category = CreateCategory(
            nameEn: "  Electronics  ",
            nameAr: "  إلكترونيات  ",
            imageKey: "  categories/electronics.jpg  ",
            descriptionEn: "  Devices  ",
            descriptionAr: "  أجهزة  ");

        // Assert
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal("Electronics", category.NameEn);
        Assert.Equal("إلكترونيات", category.NameAr);
        Assert.Equal("categories/electronics.jpg", category.ImageKey);
        Assert.Equal("Devices", category.DescriptionEn);
        Assert.Equal("أجهزة", category.DescriptionAr);
        Assert.Empty(category.Products);
    }

    [Theory]
    [InlineData(null, "إلكترونيات", "key.jpg")]
    [InlineData("", "إلكترونيات", "key.jpg")]
    [InlineData("   ", "إلكترونيات", "key.jpg")]
    [InlineData("Electronics", null, "key.jpg")]
    [InlineData("Electronics", "", "key.jpg")]
    [InlineData("Electronics", "   ", "key.jpg")]
    [InlineData("Electronics", "إلكترونيات", null)]
    [InlineData("Electronics", "إلكترونيات", "")]
    [InlineData("Electronics", "إلكترونيات", "   ")]
    public void Constructor_WithInvalidRequiredParameters_ShouldThrowDomainException(
        string? nameEn,
        string? nameAr,
        string? imageKey)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Category(nameEn!, nameAr!, imageKey!));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void Constructor_WithNullOrWhitespaceDescriptions_ShouldSetDescriptionsToNull(
        string? descriptionEn,
        string? descriptionAr)
    {
        // Act
        var category = CreateCategory(descriptionEn: descriptionEn, descriptionAr: descriptionAr);

        // Assert
        Assert.Null(category.DescriptionEn);
        Assert.Null(category.DescriptionAr);
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateAndTrim()
    {
        // Arrange
        var category = CreateCategory();

        // Act
        category.UpdateDetails("  Laptops  ", "  أجهزة حاسوب  ", "  New Desc  ", "  وصف جديد  ");

        // Assert
        Assert.Equal("Laptops", category.NameEn);
        Assert.Equal("أجهزة حاسوب", category.NameAr);
        Assert.Equal("New Desc", category.DescriptionEn);
        Assert.Equal("وصف جديد", category.DescriptionAr);
    }

    [Theory]
    [InlineData(null, "إلكترونيات")]
    [InlineData("", "إلكترونيات")]
    [InlineData("   ", "إلكترونيات")]
    [InlineData("Electronics", null)]
    [InlineData("Electronics", "")]
    [InlineData("Electronics", "   ")]
    public void UpdateDetails_WithInvalidNames_ShouldThrowDomainException(string? nameEn, string? nameAr)
    {
        // Arrange
        var category = CreateCategory();

        // Act & Assert
        Assert.Throws<DomainException>(() => category.UpdateDetails(nameEn!, nameAr!));
    }

    [Fact]
    public void UpdateImageKey_WithValidKey_ShouldUpdateAndTrim()
    {
        // Arrange
        var category = CreateCategory();

        // Act
        category.UpdateImageKey("  categories/new-image.png  ");

        // Assert
        Assert.Equal("categories/new-image.png", category.ImageKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateImageKey_WithInvalidKey_ShouldThrowDomainException(string? invalidKey)
    {
        // Arrange
        var category = CreateCategory();

        // Act & Assert
        Assert.Throws<DomainException>(() => category.UpdateImageKey(invalidKey!));
    }
}
