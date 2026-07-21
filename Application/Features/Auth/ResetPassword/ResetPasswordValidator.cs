using Application.Common.Validation;
using FluentValidation;

namespace Application.Features.Auth.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x=>x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .ValidPassword();
        }
    }
}
