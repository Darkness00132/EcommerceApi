using Application.Constants;
using Application.Features.Brands.Dtos;

public sealed record GetBrandsQuery
    : ICacheableQuery<IReadOnlyList<BrandDto>>
{
    public string CacheKey => CacheNames.Brands;

    public IReadOnlyCollection<string> Tags => [CacheNames.Brands];

    public CacheOptions CacheOptions => new()
    {
        AbsoluteExpiration = TimeSpan.FromMinutes(10)
    };
}