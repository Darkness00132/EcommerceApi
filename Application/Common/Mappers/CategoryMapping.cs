using Application.Features.Category.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappers
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, CategoryResponse>().ReverseMap();
        }
    }
}
