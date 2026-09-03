using Domain.Entities.ProcurementAggregate;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.ProcurementAggregate;

public class SupplierTests
{
    [Fact]
    public void A_Supplier_Can_Be_Created_With_Valid_Information()
    {
        // Arrange & Act
        var supplier = CreateValidSupplier();

        // Assert
        supplier.Name.Should().Be("Valid Supplier");
        supplier.ContactName.Should().Be("John Doe");
        supplier.Email.Should().Be("email@example.com");
        supplier.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void A_Supplier_Cannot_Be_Created_Without_A_Name(string name)
    {
        // Arrange & Act
        var act = () => new Supplier(name);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("Supplier name is required.");
    }

    [Fact]
    public void A_Supplier_Information_Can_Be_Changed()
    {
        // Arrange
        var supplier = CreateValidSupplier();

        // Act
        supplier.Update(
            "New Supplier",
            "Jane Doe",
            "new@example.com",
            "0123456789",
            "123 Main Street",
            "Cairo",
            "123456");

        // Assert
        supplier.Name.Should().Be("New Supplier");
        supplier.ContactName.Should().Be("Jane Doe");
        supplier.Email.Should().Be("new@example.com");
        supplier.Phone.Should().Be("0123456789");
        supplier.Address.Should().Be("123 Main Street");
        supplier.City.Should().Be("Cairo");
        supplier.TaxNumber.Should().Be("123456");
    }

    [Fact]
    public void A_Supplier_Can_Be_Deactivated()
    {
        // Arrange
        var supplier = CreateValidSupplier();

        // Act
        supplier.Deactivate();

        // Assert
        supplier.IsActive.Should().BeFalse();
    }

    [Fact]
    public void A_Deactivated_Supplier_Can_Be_Activated_Again()
    {
        // Arrange
        var supplier = CreateValidSupplier();
        supplier.Deactivate();

        // Act
        supplier.Activate();

        // Assert
        supplier.IsActive.Should().BeTrue();
    }

    private Supplier CreateValidSupplier()
    {
        return new Supplier(
            name: "Valid Supplier",
            contactName: "John Doe",
            email: "email@example.com");
    }
}
