using Domain.Enums;

namespace Application.Features.Discounts.Common;

public sealed record DiscountDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public DiscountType DiscountType { get; init; }

    public decimal Value { get; init; }

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public bool IsActive { get; init; }

    public int ProductsCount { get; init; }
}