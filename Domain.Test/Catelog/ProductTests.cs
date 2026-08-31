using System;
using Domain.Entities.Catalog;
using Domain.Entities.InventoryAggregate;
using Domain.Exceptions;
using Xunit;

namespace Domain.Test.Catalog;

public class ProductTests
{
    private static readonly Guid DefaultCategoryId = Guid.NewGuid();
    private static readonly Guid DefaultBrandId = Guid.NewGuid();

    private static Product CreateProduct(
        string nameEn = "Laptop",
        string nameAr = "حاسوب محمول",
        string descriptionEn = "High performance laptop",
        string descriptionAr = "حاسوب محمول عالي الأداء",
        string sku = "LAP-001",
        decimal price = 999.99m,
        Guid? categoryId = null,
        Guid? brandId = null)
        => new(
            nameEn,
            nameAr,
            descriptionEn,
            descriptionAr,
            sku,
            price,
            categoryId ?? DefaultCategoryId,
            brandId ?? DefaultBrandId);

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        var product = CreateProduct();

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Laptop", product.NameEn);
        Assert.Equal("حاسوب محمول", product.NameAr);
        Assert.Equal("LAP-001", product.SKU);
        Assert.Equal(999.99m, product.Price);
        Assert.False(product.IsActive);
        Assert.Empty(product.Images);
    }

    [Theory]
    [InlineData(null, "حاسوب", "DescEn", "DescAr", "SKU123")]
    [InlineData("Laptop", "", "DescEn", "DescAr", "SKU123")]
    [InlineData("Laptop", "حاسوب", "   ", "DescAr", "SKU123")]
    [InlineData("Laptop", "حاسوب", "DescEn", null, "SKU123")]
    [InlineData("Laptop", "حاسوب", "DescEn", "DescAr", "  ")]
    public void Constructor_WithInvalidTextParameters_ShouldThrowDomainException(
        string? nameEn, string? nameAr, string? descEn, string? descAr, string? sku)
    {
        Assert.Throws<DomainException>(() => new Product(
            nameEn!, nameAr!, descEn!, descAr!, sku!, 100m, DefaultCategoryId, DefaultBrandId));
    }

    [Fact]
    public void Constructor_WithNegativePrice_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Product(
            "Laptop", "حاسوب", "DescEn", "DescAr", "SKU123", -1m, DefaultCategoryId, DefaultBrandId));
    }

    [Fact]
    public void ChangePrice_WithValidPrice_ShouldUpdatePriceAndTimestamp()
    {
        var product = CreateProduct(price: 100m);

        product.ChangePrice(150m);

        Assert.Equal(150m, product.Price);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void ChangePrice_WithNegativePrice_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.ChangePrice(-10m));
    }

    [Fact]
    public void SetCategory_WithValidId_ShouldUpdateCategoryIdAndTimestamp()
    {
        var product = CreateProduct();
        var newCategoryId = Guid.NewGuid();

        product.SetCategory(newCategoryId);

        Assert.Equal(newCategoryId, product.CategoryId);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void SetCategory_WithEmptyGuid_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.SetCategory(Guid.Empty));
    }

    [Fact]
    public void SetBrand_WithValidId_ShouldUpdateBrandIdAndTimestamp()
    {
        var product = CreateProduct();
        var newBrandId = Guid.NewGuid();

        product.SetBrand(newBrandId);

        Assert.Equal(newBrandId, product.BrandId);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void SetBrand_WithEmptyGuid_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.SetBrand(Guid.Empty));
    }

    [Fact]
    public void AssignDiscount_WithValidId_ShouldSetDiscountId()
    {
        var product = CreateProduct();
        var discountId = Guid.NewGuid();

        product.AssignDiscount(discountId);

        Assert.Equal(discountId, product.DiscountId);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void AssignDiscount_WithEmptyGuid_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.AssignDiscount(Guid.Empty));
    }

    [Fact]
    public void RemoveDiscount_WhenDiscountAssigned_ShouldClearDiscountId()
    {
        var product = CreateProduct();
        product.AssignDiscount(Guid.NewGuid());

        product.RemoveDiscount();

        Assert.Null(product.DiscountId);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void AddImage_WithValidImageKey_ShouldAddToImagesCollection()
    {
        var product = CreateProduct();

        product.AddImage("  products/laptop.jpg  ");

        Assert.Single(product.Images);
        Assert.Contains(product.Images, img => img.ImageKey == "products/laptop.jpg");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddImage_WithInvalidKey_ShouldThrowDomainException(string? invalidImageKey)
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.AddImage(invalidImageKey!));
    }

    [Fact]
    public void AddImage_DuplicateKey_ShouldNotAddDuplicate()
    {
        var product = CreateProduct();
        product.AddImage("products/laptop.jpg");

        product.AddImage("products/laptop.jpg");

        Assert.Single(product.Images);
    }

    [Fact]
    public void RemoveImage_ExistingKey_ShouldRemoveFromCollection()
    {
        var product = CreateProduct();
        product.AddImage("products/laptop.jpg");

        product.RemoveImage("products/laptop.jpg");

        Assert.Empty(product.Images);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RemoveImage_WithInvalidKey_ShouldNoOp(string? invalidImageKey)
    {
        var product = CreateProduct();
        product.AddImage("products/laptop.jpg");

        product.RemoveImage(invalidImageKey!);

        Assert.Single(product.Images);
    }

    [Fact]
    public void ActivationAndDeactivation_ShouldToggleState()
    {
        var product = CreateProduct();

        product.Activate();
        Assert.True(product.IsActive);

        product.Deactivate();
        Assert.False(product.IsActive);
    }

    [Fact]
    public void SetInventory_WithValidInventory_ShouldAssignInventoryToProduct()
    {
        var product = CreateProduct();
        var inventory = new Inventory(product.Id, 50, 12);

        product.SetInventory(inventory);

        Assert.NotNull(product.Inventory);
        Assert.Equal(inventory, product.Inventory);
    }

    [Fact]
    public void SetInventory_WithNull_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.SetInventory(null!));
    }
}
