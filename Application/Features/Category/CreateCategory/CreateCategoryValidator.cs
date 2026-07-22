using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Category.CreateCategory
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.NameEn)
                    .NotEmpty()
                    .WithMessage("English name is required.")
                    .MaximumLength(100);

                RuleFor(x => x.NameAr)
                    .NotEmpty()
                    .WithMessage("Arabic name is required.")
                    .MaximumLength(100);

                RuleFor(x => x.Image)
                    .ValidImage();
            });
        }

    }
}
