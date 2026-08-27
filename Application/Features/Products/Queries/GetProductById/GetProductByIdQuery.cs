using Application.Constants;
using Application.Features.Products.Dtos;

namespace Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : ICacheableQuery<DetailedProduct>
{
    public string CacheKey => $"{CacheNames.Products}:{Id}";

    public IReadOnlyCollection<string> Tags => [CacheNames.Products];

    public CacheOptions CacheOptions => new CacheOptions {
        AbsoluteExpiration = TimeSpan.FromHours(6)
    };

    public bool BypassCache => false;
}
