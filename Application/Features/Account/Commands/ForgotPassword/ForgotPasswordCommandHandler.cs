using Application.Abstractions.Services;
using Application.Constants;
using Application.Features.Account.Dto;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Account.Commands.ForgotPassword;

internal class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IConfiguration _configuration;

    public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IBackgroundJobService backgroundJobs, IConfiguration configuration)
    {
        _userManager = userManager;
        _backgroundJobs = backgroundJobs;
        _configuration = configuration;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var frontendUrl = _configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl configuration is required.");

        var url = $"{frontendUrl.TrimEnd('/')}/account/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        var email = user.Email!;
        var model = new ForgotPasswordEmailModel(user.DisplayName, url);

        _backgroundJobs.Enqueue<IEmailSender>(sender => sender.SendAsync(email,
            "Reset your password",
            "ForgotPassword",
            model), BackgroundJobQueuesPriority.Critical);
    }
}


