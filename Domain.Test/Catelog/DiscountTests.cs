using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.Catalog;

public class DiscountTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateRange DefaultPeriod = new(Today, Today.AddDays(10));

    private static Discount CreateDiscount(
        string name = "Summer Sale",
        DiscountType type = DiscountType.Percentage,
        decimal value = 15m,
        DateRange? period = null) => new(name, type, value, period ?? DefaultPeriod);

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeCorrectly()
    {
        // Act
        var discount = CreateDiscount("  Summer Sale  ", DiscountType.FixedAmount, 50m);

        // Assert
        Assert.NotEqual(Guid.Empty, discount.Id);
        Assert.Equal("Summer Sale", discount.Name);
        Assert.Equal(DiscountType.FixedAmount, discount.DiscountType);
        Assert.Equal(50m, discount.Value);
        Assert.True(discount.IsVisible);
        Assert.Equal(DefaultPeriod.StartDate, discount.StartDate);
        Assert.Equal(DefaultPeriod.EndDate, discount.EndDate);
        Assert.Empty(discount.Products);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Discount(invalidName!, DiscountType.Percentage, 10m, DefaultPeriod));
    }

    [Theory]
    [InlineData(DiscountType.Percentage, 0)]
    [InlineData(DiscountType.Percentage, -5)]
    [InlineData(DiscountType.Percentage, 100.01)]
    [InlineData(DiscountType.FixedAmount, 0)]
    [InlineData(DiscountType.FixedAmount, -10)]
    public void ValidateValue_WithInvalidValues_ShouldThrowDomainException(DiscountType type, decimal value)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Discount("Sale", type, value, DefaultPeriod));
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var discount = CreateDiscount();
        var newPeriod = new DateRange(Today.AddDays(1), Today.AddDays(5));

        // Act
        discount.UpdateDetails("  Winter Sale  ", DiscountType.FixedAmount, 100m, newPeriod);

        // Assert
        Assert.Equal("Winter Sale", discount.Name);
        Assert.Equal(DiscountType.FixedAmount, discount.DiscountType);
        Assert.Equal(100m, discount.Value);
        Assert.Equal(newPeriod, discount.ValidityPeriod);
    }

    [Fact]
    public void ActivationAndDeactivation_ShouldToggleVisibility()
    {
        // Arrange
        var discount = CreateDiscount();

        // Act & Assert
        discount.Deactivate();
        Assert.False(discount.IsVisible);

        discount.Activate();
        Assert.True(discount.IsVisible);
    }

    [Fact]
    public void IsValidOn_WhenVisibleAndWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var period = new DateRange(Today.AddDays(-1), Today.AddDays(1));
        var discount = CreateDiscount(period: period);

        // Act & Assert
        Assert.True(discount.IsValidOn(Today));
    }

    [Fact]
    public void IsValidOn_WhenDeactivated_ShouldReturnFalse()
    {
        // Arrange
        var discount = CreateDiscount(period: DefaultPeriod);
        discount.Deactivate();

        // Act & Assert
        Assert.False(discount.IsValidOn(Today));
    }
}
