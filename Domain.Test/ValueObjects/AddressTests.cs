using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeAndTrimCorrectly()
    {
        // Act
        var address = new Address("  123 Main St  ", "  Cairo  ", "  01000000000  ", "  Ring the bell  ");

        // Assert
        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Cairo", address.City);
        Assert.Equal("01000000000", address.Phone);
        Assert.Equal("Ring the bell", address.Notes);
    }

    [Theory]
    [InlineData(null, "Cairo", "01000000000")]
    [InlineData("", "Cairo", "01000000000")]
    [InlineData("   ", "Cairo", "01000000000")]
    public void Constructor_WithInvalidStreet_ShouldThrowDomainException(string? street, string city, string phone)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Address(street!, city, phone));
        Assert.Equal("Address is required.", ex.Message);
    }

    [Theory]
    [InlineData("123 Main St", null, "01000000000")]
    [InlineData("123 Main St", "", "01000000000")]
    [InlineData("123 Main St", "   ", "01000000000")]
    public void Constructor_WithInvalidCity_ShouldThrowDomainException(string street, string? city, string phone)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Address(street, city!, phone));
        Assert.Equal("City is required.", ex.Message);
    }

    [Theory]
    [InlineData("123 Main St", "Cairo", null)]
    [InlineData("123 Main St", "Cairo", "")]
    [InlineData("123 Main St", "Cairo", "   ")]
    public void Constructor_WithInvalidPhone_ShouldThrowDomainException(string street, string city, string? phone)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Address(street, city, phone!));
        Assert.Equal("Phone is required.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespaceNotes_ShouldSetNotesToNull(string? notes)
    {
        // Act
        var address = new Address("123 Main St", "Cairo", "01000000000", notes);

        // Assert
        Assert.Null(address.Notes);
    }

    [Fact]
    public void Equality_TwoInstancesWithSameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Cairo", "01000000000", "Notes");
        var address2 = new Address("123 Main St", "Cairo", "01000000000", "Notes");

        // Act & Assert
        Assert.Equal(address1, address2);
        Assert.True(address1 == address2);
    }
}
