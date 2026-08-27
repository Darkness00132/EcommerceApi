using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Features.Categories.Dtos;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategoryById;

internal class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IRepository<Category> _categoryRepository;

    public GetCategoryByIdQueryHandler(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.ProjectToSingleOrDefaultAsync<CategoryDto>(
            category => category.Id == request.Id,
            cancellationToken);

        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);

        return category;
    }
}
