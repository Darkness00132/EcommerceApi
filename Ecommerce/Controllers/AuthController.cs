using Ecommerce.DTOs.Auth;
using Ecommerce.Services;
using Ecommerce.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        AuthService authService,
        IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Creates a new customer account and sends an email confirmation link.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        await _authService.RegisterAsync(model);

        return Ok();
    }

    /// <summary>
    /// Confirms the user's email using the confirmation link sent by email.
    /// </summary>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] Guid userId,
        [FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(userId, token);

        return Ok();
    }

    /// <summary>
    /// Authenticates the user and returns an access token and refresh token.
    /// Intended for mobile applications or clients that manage tokens themselves.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
    {
        var response = await _authService.LoginAsync(model);

        return Ok(response);
    }

    /// <summary>
    /// Authenticates the user and stores the refresh token in a secure HttpOnly cookie.
    /// Intended for browser-based applications.
    /// </summary>
    [HttpPost("login-web")]
    public async Task<ActionResult<object>> LoginWeb(LoginDto model)
    {
        var response = await _authService.LoginAsync(model);

        Response.Cookies.Append(
            "refreshToken",
            response.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(
            _jwtSettings.RefreshTokenExpirationDays),
                Path = "/"
            });

        return Ok(new
        {
            response.AccessToken,
            response.AccessTokenExpiration,
            response.Email,
            response.Roles
        });
    }

    /// <summary>
    /// Generates a new access token using a refresh token.
    /// Mobile clients should send the refresh token in the request body.
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        RefreshTokenDto model)
    {
        var response = await _authService.RefreshTokenAsync(model.RefreshToken);

        return Ok(response);
    }

    /// <summary>
    /// Generates a new access token using the refresh token stored in the HttpOnly cookie.
    /// Intended for browser-based applications.
    /// </summary>
    [HttpPost("refresh-token-web")]
    public async Task<ActionResult<AuthResponseDto>> RefreshTokenWeb()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized();

        var response = await _authService.RefreshTokenAsync(refreshToken);

        Response.Cookies.Append(
            "refreshToken",
            response.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(
            _jwtSettings.RefreshTokenExpirationDays),
                Path = "/"
            });

        return Ok(new
        {
            response.AccessToken,
            response.AccessTokenExpiration,
            response.Email,
            response.Roles
        });
    }

    /// <summary>
    /// Revokes a refresh token.
    /// Intended for mobile applications.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto model)
    {
        await _authService.LogoutAsync(model.RefreshToken);

        return Ok();
    }

    /// <summary>
    /// Revokes the refresh token stored in the HttpOnly cookie.
    /// Intended for browser-based applications.
    /// </summary>
    [HttpPost("logout-web")]
    public async Task<IActionResult> LogoutWeb()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        return Ok();
    }

    /// <summary>
    /// Sends a password reset link if the email exists.
    /// Always returns success to prevent email enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        await _authService.ForgotPasswordAsync(model.Email);

        return Ok();
    }

    /// <summary>
    /// Resets the user's password using the reset token received by email.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        await _authService.ResetPasswordAsync(model);

        return Ok();
    }
}