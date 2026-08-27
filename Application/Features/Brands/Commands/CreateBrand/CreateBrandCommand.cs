using Application.Abstractions;
using Application.Constants;

namespace Application.Features.Brands.Commands.CreateBrand;

public sealed record CreateBrandCommand(
    string NameEn,
    string NameAr
) : ICacheInvalidatingCommand<Guid>
{
    public IReadOnlyCollection<string> CacheKeys => [CacheNames.Brands];

    public IReadOnlyCollection<string> CacheTags => [];
}
