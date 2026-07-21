using Application.Common.Exceptions;
using Application.Features.Auth.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand,AuthResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthenticationResultFactory _authenticationResultFactory;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(UserManager<AppUser> userManager, AuthenticationResultFactory authenticationResultFactory, ILogger<LoginHandler> logger)
        {
            _userManager = userManager;
            _authenticationResultFactory = authenticationResultFactory;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email {Email}. User not found.", request.Email);
                throw new UnAuthorizedException("Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                _logger.LogWarning("Failed login attempt for user {UserId}.", user.Id);
                throw new UnAuthorizedException("Invalid email or password");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning(
                    "Login blocked for unconfirmed email. User {UserId}.",
                    user.Id);

                throw new UnAuthorizedException("Email is not confirmed");
            }

            return await _authenticationResultFactory.GenerateAuthResponse(user);
        }
    }
}
