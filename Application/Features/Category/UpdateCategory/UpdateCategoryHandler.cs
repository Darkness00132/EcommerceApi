using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Category.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly IImageProcessor _imageProcessor;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryHandler> _logger;

        public UpdateCategoryHandler(IStorageService storageService, IMapper mapper, IImageProcessor imageProcessor, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ILogger<UpdateCategoryHandler> logger)
        {
            _storageService = storageService;
            _mapper = mapper;
            _imageProcessor = imageProcessor;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existedCategory = await _categoryRepository.GetCategoryWithTrackingAsync(request.Id, cancellationToken);
            if (existedCategory == null)
            {
                throw new NotFoundException("category is deleted already");
            }

            string? oldImageUrl = existedCategory.ImageKey;
            _mapper.Map(request, existedCategory);

            if (request.Image is not null)
            {
                var image = await _imageProcessor.ProcessAsync(request.Image,
                    ImageType.Category);
                existedCategory.ImageKey = await _storageService.UploadAsync(image, "Categories");
            }

            await _unitOfWork.SaveChangesAsync();

            if (request.Image is not null && !string.IsNullOrEmpty(oldImageUrl))
            {
                try
                {
                    await _storageService.DeleteAsync(oldImageUrl, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete old image {OldImageUrl}", oldImageUrl);
                }
            }
        }
    }
}
