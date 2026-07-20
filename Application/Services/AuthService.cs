using Application.Common.Exceptions;
using Application.DTOs.Auth;
using Application.EmailModels;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _config;

        public AuthService(UserManager<AppUser> userManager, IMapper mapper, IJwtService jwtService, IRefreshTokenService refreshTokenService, IEmailService emailService, ILogger<AuthService> logger, IConfiguration config)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _emailService = emailService;
            _logger = logger;
            _config = config;
        }

        public async Task RegisterAsync(RegisterRequest model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
                throw new ConflictException("Email already exists");


            var user = _mapper.Map<AppUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(x => x.Description)));


            await _userManager.AddToRoleAsync(
                user,
                UserRoles.Customer.ToString());


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = $"{_config.GetValue<string>("Frontend:BaseUrl")}/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";


            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                "ConfirmEmail",
                new ConfirmEmailModel
                {
                    FirstName = user.FirstName,
                    ConfirmationLink = link
                });

            _logger.LogInformation("User {UserId} registered successfully with email {Email}.",
                user.Id,
                user.Email);
        }


        public async Task ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed. User {UserId} not found." , userId);
                throw new NotFoundException("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for user {UserId}.", user.Id);
                throw new BadRequestException("Email is already confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Invalid confirmation token for user {UserId}.", user.Id);
                throw new BadRequestException("Invalid confirmation token.");
            }
            _logger.LogInformation("Email confirmed successfully for user {UserId}.", user.Id);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email {Email}. User not found.", dto.Email);
                throw new UnAuthorizedException("Invalid email or password");
            }


            var result = await _userManager.CheckPasswordAsync(user, dto.Password);


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


            return await GenerateAuthResponse(user);
        }


        public async Task<AuthResponse> RefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenService.GetValidAsync(token);

            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid refresh token used.");

                throw new UnAuthorizedException("Invalid refresh token");
            }

            await _refreshTokenService.RevokeAsync(token);

            _logger.LogInformation("Refresh token used successfully for user {UserId}.", refreshToken.UserId);

            return await GenerateAuthResponse(refreshToken.User);
        }


        public async Task LogoutAsync(string token)
        {
            await _refreshTokenService.RevokeAsync(token);
            _logger.LogInformation("User logged out successfully.");
        }


        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation(
                    "Password reset requested for non-existing email {Email}.",
                    email);

                return;
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = $"{_config.GetValue<string>("Frontend:BaseUrl")}/auth/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(
                email,
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


        public async Task ResetPasswordAsync(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new NotFoundException("User not found");


            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.NewPassword);


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


        private async Task<AuthResponse> GenerateAuthResponse(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = await _refreshTokenService.CreateAsync(user);

            return new AuthResponse
            {
                AccessToken = _jwtService.GenerateToken(user, roles),
                RefreshToken = refreshToken.Token,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes"))
            };
        }
    }
}