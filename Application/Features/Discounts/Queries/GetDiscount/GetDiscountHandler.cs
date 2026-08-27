using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Features.Discounts.Common;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Discounts.Queries.GetDiscount;

internal class GetDiscountHandler : IRequestHandler<GetDiscountQuery, DiscountDto>
{
    private readonly IRepository<Discount> _discountRepository;

    public GetDiscountHandler(IRepository<Discount> discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<DiscountDto> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository
        .ProjectToSingleOrDefaultAsync<DiscountDto>(d => d.Id == request.Id
        , cancellationToken);

        if (discount is null)
            throw new NotFoundException($"discount not found with id: {request.Id}");

        return discount;
    }
}
