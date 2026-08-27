using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.PromotionsAggregate;

public sealed class PromoCode : AggregateRoot
{
    [MaxLength(50)]
    public string Code { get; private set; } = null!;

    public PromoDiscountType DiscountType { get; private set; }

    [Precision(18, 4)]
    public decimal Value { get; private set; }

    [Precision(18, 2)]
    public decimal MinimumOrder { get; private set; }

    public int? UsageLimit { get; private set; }

    public int UsedCount { get; private set; }

    public DateRange ValidityPeriod { get; private set; } = null!;

    public bool IsActive { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; } = [];

    public DateOnly StartDate => ValidityPeriod.StartDate;

    public DateOnly ExpirationDate => ValidityPeriod.EndDate;

    public bool HasUsageLimit => UsageLimit.HasValue;

    private PromoCode() { }

    public PromoCode(
        string code,
        PromoDiscountType discountType,
        decimal value,
        decimal minimumOrder,
        DateRange validityPeriod,
        int? usageLimit = null)
        : base(Guid.NewGuid())
    {
        Code = ValidateCode(code);

        if (minimumOrder < 0)
            throw new DomainException(
                "Minimum order cannot be negative.");

        if (usageLimit.HasValue && usageLimit.Value <= 0)
            throw new DomainException(
                "Usage limit must be greater than zero.");

        ValidateValue(discountType, value);

        DiscountType = discountType;
        Value = value;
        MinimumOrder = minimumOrder;
        ValidityPeriod = validityPeriod;
        UsageLimit = usageLimit;

        UsedCount = 0;
        IsActive = true;
    }

    public bool CanBeUsed(
        decimal orderTotal,
        DateOnly currentDate)
    {
        if (!IsActive)
            return false;

        if (!ValidityPeriod.Contains(currentDate))
            return false;

        if (orderTotal < MinimumOrder)
            return false;

        if (UsageLimit.HasValue &&
            UsedCount >= UsageLimit.Value) {
            return false;
        }

        return true;
    }

    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (orderTotal < MinimumOrder)
            return 0m;

        var discount = DiscountType switch {
            PromoDiscountType.Percentage =>
                orderTotal * Value / 100m,

            PromoDiscountType.FixedAmount =>
                Value,

            _ => throw new DomainException(
                "Invalid promo discount type.")
        };

        return Math.Min(discount, orderTotal);
    }

    public void MarkAsUsed()
    {
        if (!IsActive) {
            throw new DomainException(
                "Inactive promo code cannot be used.");
        }

        if (UsageLimit.HasValue &&
            UsedCount >= UsageLimit.Value) {
            throw new DomainException(
                "Promo code usage limit has been reached.");
        }

        UsedCount++;
    }

    public void Update(
        string code,
        PromoDiscountType discountType,
        decimal value,
        decimal minimumOrder,
        DateRange validityPeriod,
        int? usageLimit = null)
    {
        if (minimumOrder < 0) {
            throw new DomainException(
                "Minimum order cannot be negative.");
        }

        if (usageLimit.HasValue &&
            usageLimit.Value <= 0) {
            throw new DomainException(
                "Usage limit must be greater than zero.");
        }

        if (usageLimit.HasValue &&
            usageLimit.Value < UsedCount) {
            throw new DomainException(
                "Usage limit cannot be less than used count.");
        }

        ValidateValue(discountType, value);

        Code = ValidateCode(code);
        DiscountType = discountType;
        Value = value;
        MinimumOrder = minimumOrder;
        ValidityPeriod = validityPeriod;
        UsageLimit = usageLimit;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) {
            throw new DomainException(
                "Promo code is required.");
        }

        var normalizedCode =
            code.Trim().ToUpperInvariant();

        if (normalizedCode.Length > 50) {
            throw new DomainException(
                "Promo code cannot exceed 50 characters.");
        }

        return normalizedCode;
    }

    private static void ValidateValue(
        PromoDiscountType discountType,
        decimal value)
    {
        if (value <= 0) {
            throw new DomainException(
                "Promo discount value must be greater than zero.");
        }

        if (discountType == PromoDiscountType.Percentage &&
            value > 100) {
            throw new DomainException(
                "Percentage promo discount cannot exceed 100.");
        }
    }
}
