using Application.Features.Category.CreateCategory;
using Application.Features.Category.UpdateCategory;
using AutoMapper;

namespace Ecommerce.Api.Contracts.Category
{
    public class CategoryMappingConfig : Profile
    {
        public CategoryMappingConfig()
        {
            CreateMap<CreateCategoryRequest, CreateCategoryCommand>();
            CreateMap<UpdateCategoryRequest,UpdateCategoryCommand>();
        }
    }
}
