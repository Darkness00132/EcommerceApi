using FluentValidation;

namespace Application.Features.Brands.Commands.CreateBrand;

internal class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(command => command.NameEn)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.NameAr)
            .NotEmpty()
            .MaximumLength(100);
    }
}
