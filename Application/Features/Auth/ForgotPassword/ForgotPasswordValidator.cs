using FluentValidation;

namespace Application.Features.Auth.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
