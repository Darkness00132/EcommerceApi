using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Category.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand>
{
    private readonly IStorageService _storageService;
    private readonly IImageProcessor _imageProcessor;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(
        IStorageService storageService,
        IImageProcessor imageProcessor,
        IMapper mapper,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCategoryHandler> logger)
    {
        _storageService = storageService;
        _imageProcessor = imageProcessor;
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Domain.Entities.Category>(request);

        try
        {
            if (request.Image is not null)
            {
                var processedImage = await _imageProcessor.ProcessAsync(
                    request.Image,
                    ImageType.Category,
                    cancellationToken);

                category.ImageKey = await _storageService.UploadAsync(
                    processedImage,
                    "categories",
                    cancellationToken);
            }

            await _categoryRepository.CreateAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created category with ID: {CategoryId}", category.Id);
        }
        catch
        {
            // If image was uploaded before failure, delete it reliably using CancellationToken.None
            if (!string.IsNullOrEmpty(category.ImageKey))
            {
                _logger.LogWarning("Rolling back uploaded image '{ImageUrl}' due to operation failure.", category.ImageKey);

                await _storageService.DeleteAsync(category.ImageKey, CancellationToken.None);
            }
            throw;
        }
    }
}