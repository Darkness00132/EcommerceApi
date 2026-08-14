using Application.Features.Discounts.Commands.UpdateDiscount;
using AutoMapper;

namespace Ecommerce.Api.Contracts.Discount
{
    public class DiscountMapping : Profile
    {
        public DiscountMapping()
        {
            CreateMap<UpdateDiscountRequest,UpdateDiscountCommand>();
        }
    }
}
