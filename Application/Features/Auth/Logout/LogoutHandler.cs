using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<LogoutHandler> _logger;

        public LogoutHandler(IRefreshTokenService refreshTokenService, ILogger<LogoutHandler> logger)
        {
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeAsync(request.token);
            _logger.LogInformation("User logged out successfully.");
        }
    }
}
