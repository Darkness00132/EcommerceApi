using Application.Constants;
using Application.Features.Discounts.Common;

namespace Application.Features.Discounts.Queries.GetDiscount;

public record GetDiscountQuery(Guid Id) : ICacheableQuery<DiscountDto>
{
    public string CacheKey => $"{CacheNames.Discounts}:{Id}";

    public IReadOnlyCollection<string> Tags => [CacheNames.Discounts];

    public CacheOptions CacheOptions => new CacheOptions {
        AbsoluteExpiration = TimeSpan.FromDays(7)
    };

    public bool BypassCache => false;
}
