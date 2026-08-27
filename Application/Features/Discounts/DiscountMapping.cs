using Application.Features.Discounts.Common;
using AutoMapper;
using Domain.Entities.Catalog;

namespace Application.Features.Discounts;

internal class DiscountMapping : Profile
{
    public DiscountMapping()
    {
        CreateMap<Discount, DiscountDto>()
            .ForMember(d => d.StartDate, o => o.MapFrom(s => s.ValidityPeriod.StartDate))
            .ForMember(d => d.EndDate, o => o.MapFrom(s => s.ValidityPeriod.EndDate));
    }
}
