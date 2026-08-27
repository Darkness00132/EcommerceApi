using Application.Constants;
using Application.Features.Brands.Dtos;

namespace Application.Features.Brands.Queries.GetBrandById;

public sealed record GetBrandByIdQuery(Guid Id)
    : ICacheableQuery<BrandDto>
{
    public string CacheKey => $"{Constants.CacheNames.Brands}:{Id}";

    public IReadOnlyCollection<string> Tags => [Constants.CacheNames.Brands];

    public CacheOptions CacheOptions => new CacheOptions {
        AbsoluteExpiration = TimeSpan.FromDays(30),
    };

    public bool BypassCache => false;
}
