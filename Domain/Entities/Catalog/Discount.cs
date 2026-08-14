using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Catalog;

public sealed class Discount : Entity
{
    public string Name { get; private set; } = null!;

    public DiscountType DiscountType { get; private set; }

    public decimal Value { get; private set; }

    public DateRange ValidityPeriod { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    [NotMapped]
    public DateOnly StartDate => ValidityPeriod.StartDate;

    [NotMapped]
    public DateOnly EndDate => ValidityPeriod.EndDate;

    private Discount()
    {
    }

    public Discount(
        string name,
        DiscountType discountType,
        decimal value,
        DateRange validityPeriod)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Discount name is required.");

        ValidateValue(discountType, value);

        Id = Guid.NewGuid();
        Name = name.Trim();
        DiscountType = discountType;
        Value = value;
        ValidityPeriod = validityPeriod;
        IsActive = true;
    }

    public bool IsValidOn(DateOnly date)
    {
        return IsActive && ValidityPeriod.Contains(date);
    }

    public void Update(
        string name,
        DiscountType discountType,
        decimal value,
        DateRange validityPeriod)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Discount name is required.");

        ValidateValue(discountType, value);

        Name = name.Trim();
        DiscountType = discountType;
        Value = value;
        ValidityPeriod = validityPeriod;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateValue(DiscountType discountType, decimal value)
    {
        if (value <= 0)
            throw new DomainException("Discount value must be greater than zero.");

        if (discountType == DiscountType.Percentage && value > 100)
            throw new DomainException("Percentage discount cannot exceed 100.");
    }
}