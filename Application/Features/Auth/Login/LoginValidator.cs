using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Auth.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .ValidPassword();
        }
    }
}
