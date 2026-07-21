using MediatR;

namespace Application.Features.Category.DeleteCategory
{
    public record DeleteCategoryCommand(int id) : IRequest;
}
