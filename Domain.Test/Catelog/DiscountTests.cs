using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Test.Catalog;

public class DiscountTests
{
    [Fact]
    public void Discount_Created_When_Provide_Valid_Data()
    {
        // Arrange & Act
        var discount = CreateDiscount();

        // Assert
        discount.Should().NotBeNull();
        discount.DiscountType.Should().Be(DiscountType.Percentage);
        discount.Name.Should().Be("Summer Sale");
        discount.Value.Should().Be(15m);
        discount.ValidityPeriod.Should().Be(DefaultPeriod);
    }

    [Theory]
    [MemberData(nameof(InvalidDiscountData))]
    public void Discount_Creation_Fails_When_Provide_Invalid_Data(
        string name,
        DiscountType type,
        decimal value,
        DateRange period)
    {
        // Arrange & Act
        var act = () => new Discount(name, type, value, period!);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Discount_Updates_When_Provide_Valid_Data()
    {
        // Arrange
        var discount = CreateDiscount();
        var newPeriod = new DateRange(Today.AddDays(20), Today.AddDays(30));

        // Act
        discount.UpdateDetails(
            "Winter Sale",
            DiscountType.FixedAmount,
            20m,
            newPeriod);

        // Assert
        discount.Name.Should().Be("Winter Sale");
        discount.DiscountType.Should().Be(DiscountType.FixedAmount);
        discount.Value.Should().Be(20m);
        discount.ValidityPeriod.Should().Be(newPeriod);
    }

    [Theory]
    [MemberData(nameof(InvalidDiscountData))]
    public void Discount_Update_Fails_When_Provide_Invalid_Data(
        string name,
        DiscountType type,
        decimal value,
        DateRange period)
    {
        // Arrange
        var discount = CreateDiscount();

        // Act
        var act = () => discount.UpdateDetails(name, type, value, period);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Discount_Is_Active_When_Date_Is_Within_Validity_Period()
    {
        // Arrange
        var discount = CreateDiscount();

        // Act
        var result = discount.IsValidOn(Today.AddDays(5));

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void Discount_Is_Not_Active_When_Date_Is_Outside_Validity_Period(int days)
    {
        // Arrange
        var discount = CreateDiscount();

        // Act
        var result = discount.IsValidOn(Today.AddDays(days));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Discount_Is_Not_Active_When_Deactivated()
    {
        // Arrange
        var discount = CreateDiscount();
        discount.Deactivate();

        // Act
        var result = discount.IsValidOn(Today.AddDays(5));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Discount_Is_Active_When_Activated()
    {
        // Arrange
        var discount = CreateDiscount();
        discount.Deactivate();

        // Act
        discount.Activate();

        // Assert
        discount.IsValidOn(Today.AddDays(5)).Should().BeTrue();
    }

    [Fact]
    public void Discount_Calculates_Percentage_Amount()
    {
        // Arrange
        var discount = CreateDiscount(
            type: DiscountType.Percentage,
            value: 15m);

        // Act
        var result = discount.CalculateDiscountAmount(200m);

        // Assert
        result.Should().Be(30m);
    }

    [Fact]
    public void Discount_Calculates_Fixed_Amount()
    {
        // Arrange
        var discount = CreateDiscount(
            type: DiscountType.FixedAmount,
            value: 25m);

        // Act
        var result = discount.CalculateDiscountAmount(200m);

        // Assert
        result.Should().Be(25m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Discount_Calculation_Fails_When_Original_Price_Is_Invalid(decimal originalPrice)
    {
        // Arrange
        var discount = CreateDiscount();

        // Act
        var act = () => discount.CalculateDiscountAmount(originalPrice);

        // Assert
        act.Should().Throw<DomainException>();
    }

    private static readonly DateOnly Today =
        DateOnly.FromDateTime(DateTime.UtcNow);

    private static readonly DateRange DefaultPeriod =
        new(Today, Today.AddDays(10));

    public static IEnumerable<object[]> InvalidDiscountData =>
    [
        new object[] { "", DiscountType.Percentage, 15m, DefaultPeriod },
        new object[] { "   ", DiscountType.Percentage, 15m, DefaultPeriod },
        new object[] { "Summer Sale", DiscountType.Percentage, 0m, DefaultPeriod },
        new object[] { "Summer Sale", DiscountType.Percentage, -10m, DefaultPeriod },
        new object[] { "Summer Sale", DiscountType.Percentage, 101m, DefaultPeriod },
        new object[] { "Summer Sale", DiscountType.Percentage, 15m, null! }
    ];

    private Discount CreateDiscount(
        string name = "Summer Sale",
        DiscountType type = DiscountType.Percentage,
        decimal value = 15m,
        DateRange? period = null)
    {
        return new Discount(
            name,
            type,
            value,
            period ?? DefaultPeriod);
    }
}
