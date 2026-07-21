using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.ConfirmEmail
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ConfirmEmailHandler> _logger;

        public ConfirmEmailHandler(UserManager<AppUser> userManager, ILogger<ConfirmEmailHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed. User {UserId} not found.", request.UserId);
                throw new NotFoundException("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for user {UserId}.", user.Id);
                throw new BadRequestException("Email is already confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Invalid confirmation token for user {UserId}.", user.Id);
                throw new BadRequestException("Invalid confirmation token.");
            }
            _logger.LogInformation("Email confirmed successfully for user {UserId}.", user.Id);
        }
    }
}
