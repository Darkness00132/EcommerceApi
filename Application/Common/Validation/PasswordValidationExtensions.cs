using FluentValidation;

namespace Application.Common.Validation;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.");
    }
}