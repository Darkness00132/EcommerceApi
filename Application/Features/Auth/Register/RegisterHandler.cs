using Application.Common.Exceptions;
using Application.EmailModels;
using Application.Interfaces.Services;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly FrontendSettings _frontendSettings;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterHandler> _logger;

        public RegisterHandler(UserManager<AppUser> userManager, IOptions<FrontendSettings> frontendSettings, IBackgroundJobService backgroundJobService, IEmailService emailService, ILogger<RegisterHandler> logger)
        {
            _userManager = userManager;
            _frontendSettings = frontendSettings.Value;
            _backgroundJobService = backgroundJobService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);

            if (userExists != null)
                throw new ConflictException("Email already exists");


            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(x => x.Description)));


            await _userManager.AddToRoleAsync(
                user,
                UserRoles.Customer.ToString());


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = $"{_frontendSettings.BaseUrl}/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";


            _backgroundJobService.Enqueue(() => _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                "ConfirmEmail",
                new ConfirmEmailModel
                {
                    FirstName = user.FirstName,
                    ConfirmationLink = link
                }), BackgroundJopPriority.Critical);

            _logger.LogInformation("User {UserId} registered successfully with email {Email}.",
                user.Id,
                user.Email);
        }
    }
}
