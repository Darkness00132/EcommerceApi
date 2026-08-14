using Application.Constants;
using Application.Features.Discounts.Common;

namespace Application.Features.Discounts.Queries.GetDiscounts
{
    public class GetDiscountsQuery : ICacheableQuery<IReadOnlyList<DiscountDto>>
    {
        public string CacheKey => CacheNames.Discounts;

        public IReadOnlyCollection<string> Tags => [CacheNames.Discounts];

        public CacheOptions CacheOptions => new CacheOptions
        {
            AbsoluteExpiration = TimeSpan.FromDays(7)
        };
    }
}
