using Application.Abstractions.Repositories;
using Application.Features.Categories.Dtos;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategories;

internal class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IRepository<Category> _categoryRepository;

    public GetCategoriesQueryHandler(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _categoryRepository.ProjectToListAsync<CategoryDto>(
            orderBy: category => category.NameEn,
            cancellationToken: cancellationToken);
    }
}
