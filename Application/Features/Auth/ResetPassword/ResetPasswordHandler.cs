using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ResetPasswordHandler> _logger;

        public ResetPasswordHandler(UserManager<AppUser> userManager, ILogger<ResetPasswordHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new NotFoundException("User not found");


            var result = await _userManager.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword);


            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Password reset failed for user {UserId}.",
                    user.Id);

                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            _logger.LogInformation(
                "Password reset completed for user {UserId}.",
                user.Id);
        }
    }
}
