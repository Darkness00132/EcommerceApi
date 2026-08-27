using Application.Constants;
using Application.Features.Categories.Dtos;

namespace Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery
    : ICacheableQuery<IReadOnlyList<CategoryDto>>
{
    public string CacheKey => CacheNames.Categories;

    public IReadOnlyCollection<string> Tags =>
    [
        CacheNames.Categories
    ];

    public CacheOptions CacheOptions => new CacheOptions {
        AbsoluteExpiration = TimeSpan.FromDays(7)
    };

    public bool BypassCache => false;
}
