using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Constants;
using Application.Exceptions;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory;

internal class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IStorageService _storageService;
    private readonly IImageManipulationService _imageManipulationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IRepository<Category> categoryRepository, IStorageService storageService, IImageManipulationService imageManipulationService, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _storageService = storageService;
        _imageManipulationService = imageManipulationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var duplicatedCategory = await _categoryRepository.SingleOrDefaultAsync(
            category =>
                category.NameEn == request.NameEn ||
                category.NameAr == request.NameAr,
            cancellationToken);

        if (duplicatedCategory is not null)
            throw new ConflictException("A category with the same English or Arabic name already exists.");

        var manipulatedImage = await _imageManipulationService.ResizeImageAsync(request.Image, ImageType.Category, cancellationToken);
        var imageKey = await _storageService.UploadAsync(manipulatedImage, FileDestination.Categories, cancellationToken);

        var category = new Category(
            request.NameEn,
            request.NameAr,
            imageKey,
            request.DescriptionEn,
            request.DescriptionAr);

        await _categoryRepository.AddAsync(category, cancellationToken);

        try {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
        catch {
            // If saving the category fails, delete the uploaded image to avoid orphaned files.
            await _storageService.DeleteAsync(imageKey, cancellationToken);
            throw;
        }

        return category.Id;
    }
}
