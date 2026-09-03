using Domain.Entities.PromotionsAggregate;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests;

public sealed class PromoCodeTests
{
    private static readonly DateOnly Today =
        DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void A_Promo_Code_Can_Be_Created_With_Valid_Information()
    {
        // Arrange & Act
        var promoCode = CreateValidPromoCode();

        // Assert
        promoCode.Code.Should().Be("CODE");
        promoCode.DiscountType.Should().Be(PromoDiscountType.Percentage);
        promoCode.Value.Should().Be(10m);
        promoCode.MinimumOrder.Should().Be(200m);
        promoCode.UsageLimit.Should().Be(5);
        promoCode.UsedCount.Should().Be(0);
        promoCode.IsActive.Should().BeTrue();
    }

    [Fact]
    public void A_Promo_Code_Cannot_Be_Created_With_Invalid_Information()
    {
        // Arrange & Act
        var act = () => new PromoCode(
            "CODE",
            PromoDiscountType.Percentage,
            10m,
            -1m,
            CreateDateRange());

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Promo_Code_Can_Be_Used_When_The_Order_Meets_Its_Conditions()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        // Act
        var result = promoCode.CanBeUsed(300m, Today);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void A_Promo_Code_Cannot_Be_Used_When_The_Order_Does_Not_Meet_Its_Conditions()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        // Act
        var result = promoCode.CanBeUsed(100m, Today);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PromoDiscountType.Percentage, 10, 1000, 100)]
    [InlineData(PromoDiscountType.FixedAmount, 100, 1000, 100)]
    public void A_Promo_Code_Calculates_The_Correct_Discount(
        PromoDiscountType discountType,
        decimal value,
        decimal orderTotal,
        decimal expectedDiscount)
    {
        // Arrange
        var promoCode = new PromoCode(
            "CODE",
            discountType,
            value,
            0m,
            CreateDateRange());

        // Act
        var discount = promoCode.CalculateDiscount(orderTotal);

        // Assert
        discount.Should().Be(expectedDiscount);
    }

    [Fact]
    public void A_Promo_Code_Cannot_Discount_More_Than_The_Order_Total()
    {
        // Arrange
        var promoCode = new PromoCode(
            "CODE",
            PromoDiscountType.FixedAmount,
            500m,
            0m,
            CreateDateRange());

        // Act
        var discount = promoCode.CalculateDiscount(300m);

        // Assert
        discount.Should().Be(300m);
    }

    [Fact]
    public void A_Promo_Code_Counts_A_Use_When_It_Is_Used()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        // Act
        promoCode.MarkAsUsed();

        // Assert
        promoCode.UsedCount.Should().Be(1);
    }

    [Fact]
    public void A_Promo_Code_Cannot_Be_Used_After_Reaching_Its_Usage_Limit()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        promoCode.MarkAsUsed();
        promoCode.MarkAsUsed();
        promoCode.MarkAsUsed();
        promoCode.MarkAsUsed();
        promoCode.MarkAsUsed();

        // Act
        var act = () => promoCode.MarkAsUsed();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Promo_Code_Can_Be_Deactivated_And_Activated_Again()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        // Act
        promoCode.Deactivate();
        promoCode.Activate();

        // Assert
        promoCode.IsActive.Should().BeTrue();
    }

    [Fact]
    public void A_Promo_Code_Information_Can_Be_Changed()
    {
        // Arrange
        var promoCode = CreateValidPromoCode();

        // Act
        promoCode.Update(
            "NEWCODE",
            PromoDiscountType.FixedAmount,
            50m,
            300m,
            new DateRange(Today, Today.AddDays(30)),
            10);

        // Assert
        promoCode.Code.Should().Be("NEWCODE");
        promoCode.DiscountType.Should().Be(PromoDiscountType.FixedAmount);
        promoCode.Value.Should().Be(50m);
        promoCode.MinimumOrder.Should().Be(300m);
        promoCode.UsageLimit.Should().Be(10);
    }

    private PromoCode CreateValidPromoCode()
    {
        return new PromoCode(
            "code",
            PromoDiscountType.Percentage,
            10m,
            200m,
            CreateDateRange(),
            5);
    }

    private DateRange CreateDateRange()
    {
        return new DateRange(
            Today.AddDays(-1),
            Today.AddDays(30));
    }
}
