using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.Products.Dtos;

public class DiscountInProduct
{
    public Guid Id { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
