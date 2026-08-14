using Domain.Enums;

namespace Ecommerce.Api.Contracts.Discount
{
    public record UpdateDiscountRequest(string? Name,
        DiscountType? DiscountType,
        decimal? Value,
        DateOnly StartDate,
        DateOnly EndDate,
        bool? IsActive);
}
