using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Categories.Commands.CreateCategory;

internal class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.NameEn)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.NameAr)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Image)
            .ValidImageFile(5*1024*1024); // 5 MB limit

        RuleFor(command => command.DescriptionEn)
            .MaximumLength(500);

        RuleFor(command => command.DescriptionAr)
            .MaximumLength(500);
    }
}
