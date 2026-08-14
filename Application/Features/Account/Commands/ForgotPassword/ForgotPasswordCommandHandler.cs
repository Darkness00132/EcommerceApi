using Application.Features.Account.Dto;
using Application.Abstractions.Services;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Account.Commands.ForgotPassword;

internal class ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IBackgroundJobService backgroundJobs, IConfiguration configuration) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null) return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var frontendUrl = configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl configuration is required.");

        var url = $"{frontendUrl.TrimEnd('/')}/account/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        var email = user.Email!;
        var model = new PasswordResetEmailModel(user.DisplayName, url);

        backgroundJobs.Enqueue<IEmailSender>(sender => sender.SendAsync(email, "Reset your password", "ForgotPassword.cshtml", model));
    }
}


