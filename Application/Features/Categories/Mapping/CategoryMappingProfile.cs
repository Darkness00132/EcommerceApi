using Application.Features.Categories.Dtos;
using AutoMapper;
using Domain.Entities.Catalog;

namespace Application.Features.Categories.Mapping;

internal class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
