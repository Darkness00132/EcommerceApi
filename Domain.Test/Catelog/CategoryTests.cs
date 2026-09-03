using Domain.Entities.Catalog;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.Catalog;

public class CategoryTests
{
    [Fact]
    public void Category_Created_When_Provide_Valid_Data()
    {
        // Arrange & Act
        var category = CreateValidCategory();

        // Assert
        category.NameEn.Should().Be("category");
        category.NameAr.Should().Be("category");
        category.ImageKey.Should().Be("imagekey123");
        category.DescriptionEn.Should().BeNull();
        category.DescriptionAr.Should().BeNull();
    }

    [Theory]
    [InlineData("", "Category", "imagekey123")]
    [InlineData("   ", "Category", "imagekey123")]
    [InlineData("Category", "", "imagekey123")]
    [InlineData("Category", "   ", "imagekey123")]
    [InlineData("Category", "Category", "")]
    [InlineData("Category", "Category", "   ")]
    public void Category_Creation_Fails_When_Provide_Invalid_Data(
        string nameEn,
        string nameAr,
        string imageKey)
    {
        // Arrange & Act
        var act = () => new Category(nameEn, nameAr, imageKey);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Category_Updated_When_Provide_Valid_Data()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.UpdateDetails(
            "Updated Name",
            "Updated Name",
            "Updated Description",
            "Updated Description");

        // Assert
        category.NameEn.Should().Be("Updated Name");
        category.NameAr.Should().Be("Updated Name");
        category.DescriptionEn.Should().Be("Updated Description");
        category.DescriptionAr.Should().Be("Updated Description");
    }

    [Theory]
    [InlineData("", "Category")]
    [InlineData("   ", "Category")]
    [InlineData("Category", "")]
    [InlineData("Category", "   ")]
    public void Category_Update_Fails_When_Provide_Invalid_Data(
        string nameEn,
        string nameAr)
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        var act = () => category.UpdateDetails(nameEn, nameAr);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Category_Updates_Image_Key_When_Provide_Valid_Image_Key()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.UpdateImageKey("new-image-key");

        // Assert
        category.ImageKey.Should().Be("new-image-key");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Category_Image_Update_Fails_When_Provide_Invalid_Image_Key(string imageKey)
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        var act = () => category.UpdateImageKey(imageKey);

        // Assert
        act.Should().Throw<DomainException>();
    }

    private Category CreateValidCategory()
    {
        return new Category(
            "category",
            "category",
            "imagekey123");
    }
}
