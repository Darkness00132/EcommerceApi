using Application.Abstractions;
using Application.Common.Files;
using Application.Constants;

namespace Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string NameEn,
    string NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    FileDto Image) : ICacheInvalidatingCommand<Guid>
{
    public IReadOnlyCollection<string> CacheKeys => [CacheNames.Categories];

    public IReadOnlyCollection<string> CacheTags => [];
}