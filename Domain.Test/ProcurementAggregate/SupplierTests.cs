using Domain.Entities.ProcurementAggregate;
using Domain.Exceptions;

namespace Domain.Test.ProcurementAggregate;

public class SupplierTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesActiveSupplier()
    {
        var supplier = new Supplier(
            " Supplier A ",
            email: " test@test.com ");

        Assert.Equal("Supplier A", supplier.Name);
        Assert.Equal("test@test.com", supplier.Email);
        Assert.True(supplier.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsDomainException(
        string? name)
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Supplier(name!));

        Assert.Equal(
            "Supplier name is required.",
            exception.Message);
    }

    [Fact]
    public void Update_WithValidData_UpdatesProperties()
    {
        var supplier = new Supplier("Supplier A");

        supplier.Update(
            "Supplier B",
            email: "new@test.com");

        Assert.Equal("Supplier B", supplier.Name);
        Assert.Equal("new@test.com", supplier.Email);
        Assert.NotNull(supplier.UpdatedAt);
    }

    [Fact]
    public void Deactivate_SetsSupplierInactive()
    {
        var supplier = new Supplier("Supplier A");

        supplier.Deactivate();

        Assert.False(supplier.IsActive);
        Assert.NotNull(supplier.UpdatedAt);
    }

    [Fact]
    public void Activate_SetsSupplierActive()
    {
        var supplier = new Supplier("Supplier A");

        supplier.Deactivate();
        supplier.Activate();

        Assert.True(supplier.IsActive);
    }
}
