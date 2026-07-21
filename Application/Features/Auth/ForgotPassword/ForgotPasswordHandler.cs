using Application.EmailModels;
using Application.Interfaces.Services;
using Application.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Auth.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly FrontendSettings _frontendSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(UserManager<AppUser> userManager, IOptions<FrontendSettings> frontendSettings, IEmailService emailService, ILogger<ForgotPasswordHandler> logger)
        {
            _userManager = userManager;
            _frontendSettings = frontendSettings.Value;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogInformation(
                    "Password reset requested for non-existing email {Email}.",
                    request.Email);

                return;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = $"{_frontendSettings.BaseUrl}/auth/reset-password?email={request.Email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(
                request.Email,
                "Reset your password",
                "ResetPassword",
                new ResetPasswordModel
                {
                    FirstName = user.FirstName,
                    ResetLink = link
                });

            _logger.LogInformation(
                "Password reset email sent to user {UserId}.",
                user.Id);
        }
    }
}
