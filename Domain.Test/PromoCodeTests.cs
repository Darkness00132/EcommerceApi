using Domain.Entities.PromotionsAggregate;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests;

public sealed class PromoCodeTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePromoCode()
    {
        var promo = CreatePromoCode();

        Assert.Equal("SAVE10", promo.Code);
        Assert.Equal(PromoDiscountType.Percentage, promo.DiscountType);
        Assert.Equal(10m, promo.Value);
        Assert.Equal(100m, promo.MinimumOrder);
        Assert.True(promo.IsActive);
        Assert.Equal(0, promo.UsedCount);
        Assert.Null(promo.UsageLimit);
    }

    [Fact]
    public void Constructor_ShouldNormalizeCode()
    {
        var promo = new PromoCode(
            " save10 ",
            PromoDiscountType.Percentage,
            10,
            100,
            CreateDateRange());

        Assert.Equal("SAVE10", promo.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCode_ShouldThrow(string? code)
    {
        var action = () => new PromoCode(
            code!,
            PromoDiscountType.Percentage,
            10,
            100,
            CreateDateRange());

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeMinimumOrder_ShouldThrow(decimal minimumOrder)
    {
        var action = () => new PromoCode(
            "SAVE10",
            PromoDiscountType.Percentage,
            10,
            minimumOrder,
            CreateDateRange());

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_WithInvalidUsageLimit_ShouldThrow(int usageLimit)
    {
        var action = () => new PromoCode(
            "SAVE10",
            PromoDiscountType.Percentage,
            10,
            100,
            CreateDateRange(),
            usageLimit);

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidValue_ShouldThrow(decimal value)
    {
        var action = () => new PromoCode(
            "SAVE10",
            PromoDiscountType.Percentage,
            value,
            100,
            CreateDateRange());

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithPercentageGreaterThan100_ShouldThrow()
    {
        var action = () => new PromoCode(
            "SAVE10",
            PromoDiscountType.Percentage,
            101,
            100,
            CreateDateRange());

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void CanBeUsed_WhenPromoIsValid_ShouldReturnTrue()
    {
        var promo = CreatePromoCode();

        var result = promo.CanBeUsed(
            200,
            DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.True(result);
    }

    [Fact]
    public void CanBeUsed_WhenInactive_ShouldReturnFalse()
    {
        var promo = CreatePromoCode();

        promo.Deactivate();

        var result = promo.CanBeUsed(
            200,
            DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.False(result);
    }

    [Fact]
    public void CanBeUsed_WhenOrderBelowMinimum_ShouldReturnFalse()
    {
        var promo = CreatePromoCode();

        var result = promo.CanBeUsed(
            50,
            DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.False(result);
    }

    [Fact]
    public void CanBeUsed_WhenOutsideValidityPeriod_ShouldReturnFalse()
    {
        var promo = CreatePromoCode();

        var result = promo.CanBeUsed(
            200,
            promo.ExpirationDate.AddDays(1));

        Assert.False(result);
    }

    [Fact]
    public void CanBeUsed_WhenUsageLimitReached_ShouldReturnFalse()
    {
        var promo = CreatePromoCode(usageLimit: 1);

        promo.MarkAsUsed();

        var result = promo.CanBeUsed(
            200,
            DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.False(result);
    }

    [Theory]
    [InlineData(100, 10)]
    [InlineData(200, 20)]
    [InlineData(500, 50)]
    public void CalculateDiscount_WithPercentagePromo_ShouldReturnExpectedDiscount(
        decimal orderTotal,
        decimal expectedDiscount)
    {
        var promo = CreatePromoCode();

        var discount = promo.CalculateDiscount(orderTotal);

        Assert.Equal(expectedDiscount, discount);
    }

    [Fact]
    public void CalculateDiscount_WithFixedAmountPromo_ShouldReturnFixedValue()
    {
        var promo = new PromoCode(
            "SAVE50",
            PromoDiscountType.FixedAmount,
            50,
            100,
            CreateDateRange());

        var discount = promo.CalculateDiscount(200);

        Assert.Equal(50m, discount);
    }

    [Fact]
    public void CalculateDiscount_WhenDiscountExceedsOrderTotal_ShouldCapToOrderTotal()
    {
        var promo = new PromoCode(
            "SAVE500",
            PromoDiscountType.FixedAmount,
            500,
            100,
            CreateDateRange());

        var discount = promo.CalculateDiscount(200);

        Assert.Equal(200m, discount);
    }

    [Fact]
    public void CalculateDiscount_WhenOrderBelowMinimum_ShouldReturnZero()
    {
        var promo = CreatePromoCode();

        var discount = promo.CalculateDiscount(50);

        Assert.Equal(0m, discount);
    }

    [Fact]
    public void MarkAsUsed_ShouldIncrementUsedCount()
    {
        var promo = CreatePromoCode();

        promo.MarkAsUsed();

        Assert.Equal(1, promo.UsedCount);
    }

    [Fact]
    public void MarkAsUsed_WhenUsageLimitReached_ShouldThrow()
    {
        var promo = CreatePromoCode(usageLimit: 1);

        promo.MarkAsUsed();

        Assert.Throws<DomainException>(() => promo.MarkAsUsed());
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        var range = new DateRange(
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)));

        var promo = CreatePromoCode();

        promo.Update(
            "SAVE20",
            PromoDiscountType.FixedAmount,
            20,
            200,
            range,
            50);

        Assert.Equal("SAVE20", promo.Code);
        Assert.Equal(PromoDiscountType.FixedAmount, promo.DiscountType);
        Assert.Equal(20m, promo.Value);
        Assert.Equal(200m, promo.MinimumOrder);
        Assert.Equal(50, promo.UsageLimit);
        Assert.Equal(range, promo.ValidityPeriod);
    }

    [Fact]
    public void Update_WhenUsageLimitIsLessThanUsedCount_ShouldThrow()
    {
        var promo = CreatePromoCode();

        promo.MarkAsUsed();

        Assert.Throws<DomainException>(() =>
            promo.Update(
                "SAVE20",
                PromoDiscountType.Percentage,
                20,
                100,
                CreateDateRange(),
                0));
    }

    [Fact]
    public void Deactivate_ShouldSetPromoAsInactive()
    {
        var promo = CreatePromoCode();

        promo.Deactivate();

        Assert.False(promo.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetPromoAsActive()
    {
        var promo = CreatePromoCode();

        promo.Deactivate();

        promo.Activate();

        Assert.True(promo.IsActive);
    }

    private static PromoCode CreatePromoCode(int? usageLimit = null)
    {
        return new PromoCode(
            "SAVE10",
            PromoDiscountType.Percentage,
            10,
            100,
            CreateDateRange(),
            usageLimit);
    }

    private static DateRange CreateDateRange()
    {
        return new DateRange(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
    }
}
