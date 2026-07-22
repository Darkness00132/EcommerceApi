using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Category.UpdateCategory
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Image)
                .ValidImage();
        }
    }
}
