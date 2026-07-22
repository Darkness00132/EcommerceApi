using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Category.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IStorageService _storageService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryHandler> _logger;

        public DeleteCategoryHandler(IStorageService storageService, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ILogger<DeleteCategoryHandler> logger)
        {
            _storageService = storageService;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryWithTrackingAsync(request.id, cancellationToken);

            if (category == null)
            {
                _logger.LogWarning("Deletion failed: Category with ID {CategoryId} was not found", request.id);
                throw new NotFoundException("category not found");
            }

            string? imageUrlToDelete = category.ImageUrl;

            _categoryRepository.Delete(category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully deleted category with ID {CategoryId} from the database", request.id);

            if (!string.IsNullOrEmpty(imageUrlToDelete))
            {
                try
                {
                    await _storageService.DeleteAsync(imageUrlToDelete);
                    _logger.LogInformation("Successfully deleted associated image file {ImageUrl} for category ID {CategoryId}", imageUrlToDelete, request.id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Category ID {CategoryId} was deleted, but failed to delete associated image file {ImageUrl} from storage", request.id, imageUrlToDelete);
                }
            }
        }
    }
}
