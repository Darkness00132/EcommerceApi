using Domain.Entities.Catalog;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.Catalog;

public class BrandTests
{
    [Fact]
    public void Brand_Created_When_Provide_Valid_Data()
    {
        // Arrange & Act
        var brand = CreateValidBrand();

        // Assert
        brand.NameEn.Should().Be("brand");
        brand.NameAr.Should().Be("brand");
    }

    [Theory]
    [InlineData("","brand")]
    [InlineData("   ","brand")]
    [InlineData("brand","")]
    [InlineData("brand","   ")]
    public void Brand_Creation_Fails_When_Provide_Invalid_English_Or_Arabic_Name(string nameEn,string nameAr)
    {
        // Arrange & Act
        var act = () => new Brand(nameEn, nameAr);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Brand_English_And_Arabic_Name_Updated_When_Provide_Valid_Name()
    {
        // Arrange
        var brand = CreateValidBrand();

        // Act
        brand.UpdateEnglishName("Updated Brand");
        brand.UpdateArabicName("Updated Brand");

        // Assert
        brand.NameEn.Should().Be("Updated Brand");
        brand.NameAr.Should().Be("Updated Brand");
    }

    [Theory]
    [InlineData("", "brand")]
    [InlineData("   ", "brand")]
    [InlineData("brand", "")]
    [InlineData("brand", "   ")]
    public void Brand_English_Name_Update_Fails_When_Provide_Invalid_Name(string nameEn,string nameAr)
    {
        // Arrange
        var brand = CreateValidBrand();

        // Act
        var act = () => {
            brand.UpdateEnglishName(nameEn);
            brand.UpdateArabicName(nameAr);
        };

        // Assert
        act.Should().Throw<DomainException>();
    }

    private Brand CreateValidBrand()
    {
        return new Brand("brand", "brand");
    }
}
