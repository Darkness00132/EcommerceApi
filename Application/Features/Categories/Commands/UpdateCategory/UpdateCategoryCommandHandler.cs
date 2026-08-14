using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Constants;
using Application.Exceptions;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory;

internal class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IImageManipulationService _imageReferenceService;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IRepository<Category> categoryRepository, IImageManipulationService imageReferenceService, IStorageService storageService, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _imageReferenceService = imageReferenceService;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);

        var duplicatedCategory = await _categoryRepository.SingleOrDefaultAsync(
            existingCategory => existingCategory.Id != request.Id,
            cancellationToken);

        if (duplicatedCategory is not null)
            throw new ConflictException("A category with the same English or Arabic name already exists.");

        string? newImageKey = null;

        if (request.NewImage is not null && request.NewImage.Length > 0) 
        {
            var manipulatedImage = await _imageReferenceService.ResizeImage(request.NewImage,ImageType.Category);
            newImageKey = await _storageService.UploadAsync(manipulatedImage, FileDestination.Categories);
        }

        category.Update(
            nameEn:request.NameEn,
            nameAr:request.NameAr,
            imageKey:newImageKey,
            descriptionEn:request.DescriptionEn,
            descriptionAr:request.DescriptionAr);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if(newImageKey is not null)
                await _storageService.DeleteAsync(newImageKey);
            throw;
        }
    }
}
