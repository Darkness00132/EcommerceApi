using Application.Abstractions.Repositories;
using Application.Features.Brands.Dtos;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Brands.Queries.GetBrands;

internal class GetBrandsQueryHandler
    : IRequestHandler<GetBrandsQuery, IReadOnlyList<BrandDto>>
{
    private readonly IRepository<Brand> _brandRepository;

    public GetBrandsQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IReadOnlyList<BrandDto>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        return await _brandRepository.ProjectToListAsync<BrandDto>(
            orderBy: brand => brand.NameEn,
            cancellationToken: cancellationToken);
    }
}
