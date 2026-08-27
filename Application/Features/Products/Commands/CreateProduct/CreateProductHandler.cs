using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Constants;
using Application.Exceptions;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

internal class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<Brand> _brandRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Discount> _discountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public CreateProductHandler(IProductRepository productRepository, IRepository<Brand> brandRepository, IRepository<Category> categoryRepository, IRepository<Discount> discountRepository, IUnitOfWork unitOfWork, IStorageService storageService)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _discountRepository = discountRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!await _categoryRepository.ExistsAsync(x => x.Id == request.CategoryId)) {
            throw new NotFoundException($"Category with id {request.CategoryId} doesnot exist");
        }

        if (!await _brandRepository.ExistsAsync(x => x.Id == request.BrandId)) {
            throw new NotFoundException($"Brand with id {request.BrandId} doesnot exist");
        }

        var product = new Product(request.NameEn,
            request.NameAr,
            request.DescriptionEn,
            request.DescriptionAr,
            request.SKU,
            request.Price,
            request.CategoryId,
            request.BrandId);

        if (request.IsVisible)
            product.Activate();

        if (request.DiscountId is not null) {
            if (await _discountRepository.ExistsAsync(d => d.Id == request.DiscountId)) {
                product.AssignDiscount(request.DiscountId.Value);
            }
        }

        var images = await _storageService
            .UploadManyAsync(request.Images,
            FileDestination.Products,
            cancellationToken);

        foreach (var image in images)
            product.AddImage(image);

        await _productRepository.AddAsync(product, cancellationToken);
        try {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch {
            await _storageService.DeleteManyAsync(images);
        }

        return product.Id;
    }
}
