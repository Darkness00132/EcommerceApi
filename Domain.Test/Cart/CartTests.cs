using Domain.Entities.Carts;
using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Test.Carts;

public class CartTests
{
    [Fact]
    public void Cart_Add_Item_When_Provide_Valid_Data()
    {
        // Arrange
        var cart = CreateValidCart();
        var product = CreateValidProduct();

        // Act
        cart.AddItem(product.Id, 2);

        // Assert
        cart.Items.Should().ContainSingle(x =>
            x.ProductId == product.Id &&
            x.Quantity == 2
        );
    }

    [Theory]
    [MemberData(nameof(InvalidCartItems))]
    public void AddItem_WhenInvalidData_ThrowsDomainException(
        Guid productId,
        int quantity)
    {
        // Arrange
        var cart = CreateValidCart();

        // Act
        var act = () => cart.AddItem(productId, quantity);

        // Assert
        act.Should().Throw<DomainException>();
        cart.Items.Should().BeEmpty();
    }


    [Fact]
    public void Cart_Update_Quantity_Of_Item()
    {
        // Arrange
        var cart = CreateValidCart();
        var product = CreateValidProduct();
        cart.AddItem(product.Id, 2);

        // Act
        cart.UpdateItemQuantity(product.Id, 5);

        // Assert
        cart.Items.Should().ContainSingle(x =>
            x.ProductId == product.Id &&
            x.Quantity == 5
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Cart_Update_Quantity_Should_Fail_When_Supply_Invalid_Quantity(int quantity)
    {
        // Arrange
        var cart = CreateValidCart();
        var product = CreateValidProduct();
        cart.AddItem(product.Id, 2);

        // Act
        var act = () => cart.UpdateItemQuantity(product.Id, quantity);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cart_Removes_Item_When_Valid_Product()
    {
        // Arrange
        var cart = CreateValidCart();
        var product = CreateValidProduct();
        cart.AddItem(product.Id, 2);

        // Act
        cart.RemoveItem(product.Id);

        // Assert
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void Cart_Does_Not_Remove_Item_When_Provide_Nonexistent_Product()
    {
        // Arrange
        var cart = CreateValidCart();
        var product = CreateValidProduct();
        cart.AddItem(product.Id, 2);

        // Act
        cart.RemoveItem(Guid.NewGuid());

        // Assert
        cart.Items.Should().HaveCount(1);
    }


    public static IEnumerable<object[]> InvalidCartItems =>
    [
        new object[] { Guid.Empty, 1 },
        new object[] { Guid.NewGuid(), 0 },
        new object[] { Guid.NewGuid(), -1 }
    ];

    private Product CreateValidProduct()
    {
        return new Product("product", "product", "product that is product", "product that is product", "SKU-123", 100, Guid.NewGuid(), Guid.NewGuid());
    }

    private Cart CreateValidCart()
    {
        var user = new AppUser(new FullName("mohamed", "tarek"), "test@example.com");
        return new Cart(user.Id);
    }
}
