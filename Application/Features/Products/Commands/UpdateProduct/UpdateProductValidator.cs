using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x)
            .HasAtLeastOneValue(nameof(UpdateProductCommand.Id));

        RuleForEach(x => x.NewImages)
            .ValidImageFile(10 * 1024 * 1024)
            .When(x => x.NewImages is not null);

        RuleFor(x => x.NewImages)
            .Must(images => images!.Count is >= 1 and <= 5)
            .When(x => x.NewImages is not null);

        RuleFor(x => x.DeletedImages)
            .Must(images => images!.Count is >= 1 and <= 5)
            .When(x => x.DeletedImages is not null);

        RuleFor(x => x)
            .Must(x =>
                x.NewImages!.Count + x.DeletedImages!.Count <= 5)
            .When(x =>
                x.NewImages is not null &&
                x.DeletedImages is not null);
    }
}
