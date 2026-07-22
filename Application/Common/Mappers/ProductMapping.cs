using Application.Features.Product.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappers
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductItemResponse>().ReverseMap();
        }
    }
}
