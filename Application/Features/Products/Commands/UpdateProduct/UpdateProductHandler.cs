using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Common.Files;
using Application.Constants;
using Application.Exceptions;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

internal class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<Brand> _brandRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageManipulationService _imageManipulationService;
    private readonly IStorageService _storageService;

    public UpdateProductHandler(IProductRepository productRepository, IRepository<Brand> brandRepository, IRepository<Category> categoryRepository, IUnitOfWork unitOfWork, IImageManipulationService imageManipulationService, IStorageService storageService)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _imageManipulationService = imageManipulationService;
        _storageService = storageService;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository
            .SingleOrDefaultAsync(p => p.Id == request.Id,
            cancellationToken,
            p => p.Brand, p => p.Category, p => p.Images);

        if (product == null)
            throw new NotFoundException($"Product with Id {request.Id} doesnot exist");

        if (request.CategoryId is not null &&
            !await _categoryRepository.ExistsAsync(x => x.Id == request.CategoryId, cancellationToken)) {
            throw new NotFoundException($"Category with id {request.CategoryId} doesnot exist");
        }

        if (request.BrandId is not null &&
            !await _brandRepository.ExistsAsync(x => x.Id == request.BrandId, cancellationToken)) {
            throw new NotFoundException($"Brand with id {request.BrandId} doesnot exist");
        }

        product.UpdateDetails(
            request.NameEn ?? product.NameEn,
            request.NameAr ?? product.NameAr,
            request.DescriptionEn ?? product.DescriptionEn,
            request.DescriptionAr ?? product.DescriptionAr,
            request.SKU ?? product.SKU);

        if (request.DeletedImages is not null) {
            request.DeletedImages.ForEach(i => product.RemoveImage(i));
        }

        List<string> imagesToBeUploaded = new();
        if (request.NewImages is not null) {
            var resizedImagesToGo = new List<Task<FileDto>>();
            request.NewImages.ForEach(async i => {
                resizedImagesToGo.Add(_imageManipulationService.ResizeImageAsync(i, ImageType.Product, cancellationToken));
            });
            var resizedImages = await Task.WhenAll(resizedImagesToGo);

            var images = await _storageService.UploadManyAsync(resizedImages, FileDestination.Products, cancellationToken);
            imagesToBeUploaded.AddRange(images);
            imagesToBeUploaded.ForEach(i => product.AddImage(i));
        }
        try {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch {
            await _storageService.DeleteManyAsync(imagesToBeUploaded);
        }
    }
}
