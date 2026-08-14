using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Brands.Commands.CreateBrand;

internal class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Guid>
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandCommandHandler(
        IRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var duplicatedBrand = await _brandRepository.SingleOrDefaultAsync(
            brand => brand.NameEn == request.NameEn || brand.NameAr == request.NameAr,
            cancellationToken);

        if (duplicatedBrand is not null)
            throw new ConflictException("A brand with the same English or Arabic name already exists.");

        var brand = new Brand(
            request.NameEn,
            request.NameAr);

        await _brandRepository.AddAsync(brand, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}
