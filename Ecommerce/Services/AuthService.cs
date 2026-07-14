using AutoMapper;
using Ecommerce.Common.Exceptions;
using Ecommerce.Data.Entities;
using Ecommerce.Data.Enums;
using Ecommerce.DTOs.Auth;
using Ecommerce.Settings;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly IEmailSender<AppUser> _emailSender;
        private readonly FrontendSettings _frontend;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, JwtService jwtService, RefreshTokenService refreshTokenService, IEmailSender<AppUser> emailSender, FrontendSettings frontend, JwtSettings jwtSettings, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _emailSender = emailSender;
            _frontend = frontend;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }
        public async Task RegisterAsync(RegisterDto model)
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

            var link = $"{_frontend.BaseUrl}/confirm-email?userId={user.Id}&token={token}";


            await _emailSender.SendConfirmationLinkAsync(
                user,
                user.Email!,
                link);

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
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email {Email}. User not found.", dto.Email);
                throw new UnAuthorizedException("Invalid email or password");
            }


            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                false);


            if (!result.Succeeded)
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


        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenService.GetValidAsync(token);

            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid refresh token used.");

                throw new UnAuthorizedException("Invalid refresh token");
            }

            await _refreshTokenService.RevokeAsync(token);

            _logger.LogInformation(
                "Refresh token used successfully for user {UserId}.",
                refreshToken.UserId);


            await _refreshTokenService.RevokeAsync(token);


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

            var link = $"{_frontend.BaseUrl}/reset-password?email={email}&token={token}";


            await _emailSender.SendPasswordResetLinkAsync(
                user,
                email,
                link);

            _logger.LogInformation(
                "Password reset email sent to user {UserId}.",
                user.Id);
        }


        public async Task ResetPasswordAsync(ResetPasswordDto model)
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


        private async Task<AuthResponseDto> GenerateAuthResponse(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = await _refreshTokenService.CreateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = _jwtService.GenerateToken(user, roles),
                RefreshToken = refreshToken.Token,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Email = user.Email!,
                Roles = roles
            };
        }
    }
}