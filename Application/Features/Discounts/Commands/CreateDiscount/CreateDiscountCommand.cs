using Application.Abstractions;
using Application.Constants;
using Domain.Enums;

namespace Application.Features.Discounts.Commands.CreateDiscount
{
    public record CreateDiscountCommand(string Name,
        DiscountType DiscountType,
        decimal Value,
        DateOnly StartDate,
        DateOnly EndDate,
        bool IsActive) : ICacheInvalidatingCommand<Guid>
    {
        public IReadOnlyCollection<string> CacheKeys => [CacheNames.Discounts];

        public IReadOnlyCollection<string> CacheTags => [];
    }
}
