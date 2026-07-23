using Application.Features.Category.CreateCategory;
using Application.Features.Category.Dtos;
using Application.Features.Category.UpdateCategory;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappers
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ImageUrl,opt => opt.MapFrom(src=>src.ImageKey));

            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>()
                .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null));
        }
    }
}
