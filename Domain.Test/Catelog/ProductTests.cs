using System.Security.Cryptography.X509Certificates;
using Domain.Entities.Catalog;
using Domain.Entities.InventoryAggregate;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Test.Catalog;

public class ProductTests
{
    [Fact]
    public void Product_Created_When_Provide_Valid_Data()
    {
        // Arrange & Act
        var product = CreateValidProduct();

        // Assert
        product.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(InvalidProductData))]
    public void Product_Not_Created_When_Provide_Invalid_Data(
        string nameEn,
        string nameAr,
        string descriptionEn,
        string descriptionAr,
        string sku,
        decimal price,
        Guid categoryId,
        Guid brandId)
    {
        // Arrange & Act
        var act = () => new Product(
            nameEn,
            nameAr,
            descriptionEn,
            descriptionAr,
            sku,
            price,
            categoryId,
            brandId);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Product_Updated_When_Provide_Valid_Data()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.UpdateDetails("Updated Name", "Updated Name", "Updated Description", "Updated Description", "SKU-456");

        // Assert
        product.NameEn.Should().Be("Updated Name");
        product.NameAr.Should().Be("Updated Name");
        product.DescriptionEn.Should().Be("Updated Description");
        product.DescriptionAr.Should().Be("Updated Description");
        product.SKU.Should().Be("SKU-456");
    }

    [Theory]
    [InlineData("", "Name", "Description", "Description", "SKU-123")]
    [InlineData("Name", "", "Description", "Description", "SKU-123")]
    [InlineData("Name", "Name", "", "Description", "SKU-123")]
    [InlineData("Name", "Name", "Description", "", "SKU-123")]
    [InlineData("Name", "Name", "Description", "Description", "")]
    public void Product_Not_Updated_When_Provide_Invalid_Data(
        string nameEn,
        string nameAr,
        string descriptionEn,
        string descriptionAr,
        string sku)
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var act = () => product.UpdateDetails(
            nameEn,
            nameAr,
            descriptionEn,
            descriptionAr,
            sku);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Product_Assign_Discount_When_Provide_Valid_Discount()
    {
        // Arrange
        var product = CreateValidProduct();
        var discount = CreateValidDiscount();

        // Act
        product.AssignDiscount(discount);

        // Assert
        product.Discount.Should().Be(discount);
    }

    [Fact]
    public void Product_Assign_Inventory_When_Provide_Valid_Inventory()
    {
        // Arrange
        var product = CreateValidProduct();
        var inventory = CreateValidInventory();

        // Act
        product.SetInventory(inventory);

        // Assert
        product.Inventory.Should().Be(inventory);
    }

    [Fact]
    public void Product_Change_Price_When_Provide_Valid_Price()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.ChangePrice(200);

        // Assert
        product.Price.Should().Be(200);
    }

    [Fact]
    public void Product_Not_Change_Price_When_Provide_Negative_Price()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var act = () => product.ChangePrice(-1);

        // Assert
        act.Should().Throw<DomainException>();
        product.Price.Should().Be(100);
    }

    [Fact]
    public void Product_Set_Category_When_Provide_Valid_Category()
    {
        // Arrange
        var product = CreateValidProduct();
        var categoryId = Guid.NewGuid();

        // Act
        product.SetCategory(categoryId);

        // Assert
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Product_Remove_Discount()
    {
        // Arrange
        var product = CreateValidProduct();
        var discount = CreateValidDiscount();
        product.AssignDiscount(discount);

        // Act
        product.RemoveDiscount();

        // Assert
        product.DiscountId.Should().BeNull();
    }

    [Fact]
    public void Product_Add_Image_When_Provide_Valid_Image_Key()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.AddImage("image-123");

        // Assert
        product.Images.Should().ContainSingle(x => x.ImageKey == "image-123");
    }

    [Fact]
    public void Product_Does_Not_Add_Duplicate_Image()
    {
        // Arrange
        var product = CreateValidProduct();

        product.AddImage("image-123");

        // Act
        product.AddImage("image-123");

        // Assert
        product.Images.Should().ContainSingle();
    }

    [Fact]
    public void Product_Remove_Image_When_Image_Exists()
    {
        // Arrange
        var product = CreateValidProduct();
        product.AddImage("image-123");

        // Act
        product.RemoveImage("image-123");

        // Assert
        product.Images.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Not_Add_Image_When_Image_Key_Is_Invalid(string imageKey)
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var act = () => product.AddImage(imageKey);

        // Assert
        act.Should().Throw<DomainException>();
        product.Images.Should().BeEmpty();
    }

    [Fact]
    public void Product_Activate()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Product_Deactivate()
    {
        // Arrange
        var product = CreateValidProduct();
        product.Activate();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
    }

    public static IEnumerable<object[]> InvalidProductData =>
    [
        new object[] { null!, "Name", "Description", "Description", "SKU-123", 100, Guid.NewGuid(), Guid.NewGuid() },

        new object[] { "", "Name", "Description", "Description", "SKU-123", 100, Guid.NewGuid(), Guid.NewGuid() },

        new object[] { "Name", "Name", "Description", "Description", "SKU-123", -1, Guid.NewGuid(), Guid.NewGuid() },

        new object[] { "Name", "Name", "Description", "Description", "SKU-123", 100, Guid.Empty, Guid.NewGuid() },

        new object[] { "Name", "Name", "Description", "Description", "SKU-123", 100, Guid.NewGuid(), Guid.Empty }
    ];

    private Product CreateValidProduct()
    {
        return new Product("product", "product", "product that is product", "product that is product", "SKU-123", 100, Guid.NewGuid(), Guid.NewGuid());
    }

    private Discount CreateValidDiscount()
    {
        var dateRange = new DateRange(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)));

        return new Discount("Test Account",DiscountType.Percentage,10,dateRange);
    }

    private Inventory CreateValidInventory() 
    {
        return new Inventory(Guid.NewGuid(), 100, 10);
    }
}
