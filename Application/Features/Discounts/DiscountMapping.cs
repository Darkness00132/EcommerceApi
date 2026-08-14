using Application.Features.Discounts.Common;
using AutoMapper;
using Domain.Entities.Catalog;

namespace Application.Features.Discounts
{
    internal class DiscountMapping : Profile
    {
        public DiscountMapping()
        {
            CreateMap<Discount, DiscountDto>();
        }
    }
}
