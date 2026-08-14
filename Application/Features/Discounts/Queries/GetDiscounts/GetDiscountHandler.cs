using Application.Abstractions.Repositories;
using Application.Features.Discounts.Common;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Discounts.Queries.GetDiscounts
{
    internal class GetDiscountHandler : IRequestHandler<GetDiscountsQuery, IReadOnlyList<DiscountDto>>
    {
        private readonly IRepository<Discount> _discountRepository;

        public GetDiscountHandler(IRepository<Discount> discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IReadOnlyList<DiscountDto>> Handle(GetDiscountsQuery request, CancellationToken cancellationToken)
            => await _discountRepository
            .ProjectToPagedAsync<DiscountDto>(d=>d.StartDate,cancellationToken:cancellationToken);
    }
}