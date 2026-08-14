using Application.Abstractions.Repositories;
using Application.Features.Discounts.Commands.DeleteDiscount;
using Domain.Entities.Catalog;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Discounts.Commands.DeleteDiscount
{
    internal class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand>
    {
        private readonly IRepository<Discount> _discountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDiscountHandler(IRepository<Discount> discountRepository, IUnitOfWork unitOfWork)
        {
            _discountRepository = discountRepository;
            _unitOfWork = unitOfWork;
        }

        public Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

internal sealed class DeleteDiscountHandler
    : IRequestHandler<DeleteDiscountCommand>
{
    private readonly IRepository<Discount> _discountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountHandler(
        IRepository<Discount> discountRepository,
        IUnitOfWork unitOfWork)
    {
        _discountRepository = discountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (discount is null)
        {
            throw new DomainException("Discount was not found.");
        }

        _discountRepository.Remove(discount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}