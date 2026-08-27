using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.ValueObjects;

public class FullNameTests
{
    [Fact]
    public void Constructor_WithValidNames_ShouldInitializeAndTrimCorrectly()
    {
        // Act
        var fullName = new FullName("  John  ", "  Doe  ");

        // Assert
        Assert.Equal("John", fullName.FirstName);
        Assert.Equal("Doe", fullName.LastName);
        Assert.Equal("John Doe", fullName.ToString());
    }

    [Theory]
    [InlineData(null, "Doe")]
    [InlineData("", "Doe")]
    [InlineData("   ", "Doe")]
    public void Constructor_WithInvalidFirstName_ShouldThrowDomainException(string? firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new FullName(firstName!, lastName));
    }

    [Theory]
    [InlineData("John", null)]
    [InlineData("John", "")]
    [InlineData("John", "   ")]
    public void Constructor_WithInvalidLastName_ShouldThrowDomainException(string firstName, string? lastName)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new FullName(firstName, lastName!));
    }

    [Fact]
    public void Equality_TwoInstancesWithSameValues_ShouldBeEqual()
    {
        // Arrange
        var name1 = new FullName("John", "Doe");
        var name2 = new FullName("John", "Doe");

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.True(name1 == name2);
    }
}
