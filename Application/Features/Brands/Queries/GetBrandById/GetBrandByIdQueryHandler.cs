using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Features.Brands.Dtos;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Brands.Queries.GetBrandById;

internal class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto>
{
    private readonly IRepository<Brand> _brandRepository;

    public GetBrandByIdQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandDto> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (brand is null)
            throw new NotFoundException(nameof(Brand), request.Id);

        return new BrandDto(
            brand.Id,
            brand.NameEn,
            brand.NameAr);
    }
}
