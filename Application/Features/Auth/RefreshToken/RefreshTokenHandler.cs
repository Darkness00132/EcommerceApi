using Application.Common.Exceptions;
using Application.Features.Auth.Dtos;
using Application.Interfaces.Services;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly AuthenticationResultFactory _authenticationResultFactory;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(IRefreshTokenService refreshTokenService, AuthenticationResultFactory authenticationResultFactory, ILogger<RefreshTokenHandler> logger)
        {
            _refreshTokenService = refreshTokenService;
            _authenticationResultFactory = authenticationResultFactory;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenService.GetValidAsync(request.token);

            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid refresh token used.");

                throw new UnAuthorizedException("Invalid refresh token");
            }

            await _refreshTokenService.RevokeAsync(request.token);

            _logger.LogInformation("Refresh token used successfully for user {UserId}.", refreshToken.UserId);

            return await _authenticationResultFactory.GenerateAuthResponse(refreshToken.User);
        }
    }
}
