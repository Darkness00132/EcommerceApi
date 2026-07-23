using Application.Features.Product.CreateProduct;
using Application.Features.Product.UpdateProduct;
using AutoMapper;

namespace Ecommerce.Api.Contracts.Product
{
    public class ProductMappingConfig : Profile
    {
        public ProductMappingConfig()
        {
            CreateMap<CreateProductRequest, CreateProductCommand>();
            CreateMap<UpdateProductRequest, UpdateProductCommand>();
        }
    }
}
