using FluentValidation;

namespace Application.Features.Account.Commands.Register;

internal class RegisterAccountValidator : AbstractValidator<RegisterAccountCommand>
{
    public RegisterAccountValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
