using Application.Features.Brands.Dtos;
using AutoMapper;
using Domain.Entities.Catalog;

namespace Application.Features.Brands.Mapping;

internal class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>();
    }
}
