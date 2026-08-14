using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.PromotionsAggregate;

public sealed class PromoCode : AggregateRoot
{
    public string Code { get; private set; } = null!;

    public PromoDiscountType DiscountType { get; private set; }

    public decimal Value { get; private set; }

    public decimal MinimumOrder { get; private set; }

    public int? UsageLimit { get; private set; }

    public int UsedCount { get; private set; }

    public DateRange ValidityPeriod { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    [NotMapped]
    public DateOnly StartDate => ValidityPeriod.StartDate;

    [NotMapped]
    public DateOnly ExpirationDate => ValidityPeriod.EndDate;

    [NotMapped]
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
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Promo code is required.");

        if (minimumOrder < 0)
            throw new DomainException("Minimum order cannot be negative.");

        if (usageLimit.HasValue && usageLimit.Value <= 0)
            throw new DomainException("Usage limit must be greater than zero.");

        ValidateValue(discountType, value);

        Code = code.Trim().ToUpperInvariant();
        DiscountType = discountType;
        Value = value;
        MinimumOrder = minimumOrder;
        ValidityPeriod = validityPeriod;
        UsageLimit = usageLimit;
        UsedCount = 0;
        IsActive = true;
    }

    public bool CanBeUsed(decimal orderTotal, DateOnly date)
    {
        if (!IsActive)
            return false;

        if (!ValidityPeriod.Contains(date))
            return false;

        if (orderTotal < MinimumOrder)
            return false;

        if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
            return false;

        return true;
    }

    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (orderTotal < MinimumOrder)
            return 0;

        var discount = DiscountType switch
        {
            PromoDiscountType.Percentage => orderTotal * Value / 100,
            PromoDiscountType.FixedAmount => Value,
            _ => throw new DomainException("Invalid promo discount type.")
        };

        return discount > orderTotal ? orderTotal : discount;
    }

    public void MarkAsUsed()
    {
        if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
            throw new DomainException("Promo code usage limit has been reached.");

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
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Promo code is required.");

        if (minimumOrder < 0)
            throw new DomainException("Minimum order cannot be negative.");

        if (usageLimit.HasValue && usageLimit.Value <= 0)
            throw new DomainException("Usage limit must be greater than zero.");

        if (usageLimit.HasValue && usageLimit.Value < UsedCount)
            throw new DomainException("Usage limit cannot be less than used count.");

        ValidateValue(discountType, value);

        Code = code.Trim().ToUpperInvariant();
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

    private static void ValidateValue(PromoDiscountType discountType, decimal value)
    {
        if (value <= 0)
            throw new DomainException("Promo discount value must be greater than zero.");

        if (discountType == PromoDiscountType.Percentage && value > 100)
            throw new DomainException("Percentage promo discount cannot exceed 100.");
    }
}