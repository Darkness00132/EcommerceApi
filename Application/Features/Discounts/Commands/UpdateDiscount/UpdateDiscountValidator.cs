using Application.Common.Validation;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.Discounts.Commands.UpdateDiscount;

internal sealed class UpdateDiscountValidator
    : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountValidator()
    {
        RuleFor(x => x)
            .HasAtLeastOneValue(nameof(UpdateDiscountCommand.Id));

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .When(x => x.Value is not null);

        RuleFor(x => x.Value)
            .LessThanOrEqualTo(100)
            .WithMessage("Percentage discount cannot exceed 100.")
            .When(x =>
                x.DiscountType == DiscountType.Percentage &&
                x.Value is not null);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.")
            .When(x =>
                x.StartDate is not null &&
                x.EndDate is not null);

        RuleFor(x => x.DiscountType)
            .IsInEnum()
            .When(x => x.DiscountType is not null);
    }
}
