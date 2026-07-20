using Application.DTOs.Auth;
using Ecommerce.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtSettings _jwtSettings;
    private readonly IAntiforgery _antiforgery;

    public AuthController(AuthService authService, IOptions<JwtSettings> jwtSettings, IAntiforgery antiforgery)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
        _antiforgery = antiforgery;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// The account requires email confirmation before login.
    /// </remarks>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        await _authService.RegisterAsync(model);

        return Ok();
    }

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="token">Email confirmation token.</param>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] Guid userId,
        [FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(userId, token);

        return Ok();
    }


    /// <summary>
    /// Authenticates a user and returns access and refresh tokens.
    /// </summary>
    /// <returns>Authentication tokens.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest model)
    {
        var response = await _authService.LoginAsync(model);

        return Ok(response);
    }


    /// <summary>
    /// Authenticates a web user and creates a refresh token cookie.
    /// </summary>
    /// <remarks>
    /// Designed for browser-based clients.
    /// </remarks>
    [HttpPost("login-web")]
    public async Task<ActionResult<WebAuthResponse>> LoginWeb(LoginRequest model)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

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

        return Ok(new WebAuthResponse
        {
            AccessToken = response.AccessToken,
            AccessTokenExpiration = response.AccessTokenExpiration
        });
    }


    /// <summary>
    /// Refreshes access tokens using a refresh token.
    /// </summary>
    /// <remarks>
    /// Used by clients that manage refresh tokens manually.
    /// </remarks>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        RefreshTokenRequest model)
    {
        var response =
            await _authService.RefreshTokenAsync(model.RefreshToken);

        return Ok(response);
    }


    /// <summary>
    /// Refreshes access tokens using the refresh token cookie.
    /// </summary>
    /// <remarks>
    /// Used by browser-based clients.
    /// </remarks>
    [HttpPost("refresh-token-web")]
    public async Task<ActionResult<WebAuthResponse>> RefreshTokenWeb()
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        if (!Request.Cookies.TryGetValue(
            "refreshToken",
            out var refreshToken))
        {
            return Unauthorized();
        }

        var response =
            await _authService.RefreshTokenAsync(refreshToken);

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

        return Ok(new WebAuthResponse
        {
            AccessToken = response.AccessToken,
            AccessTokenExpiration = response.AccessTokenExpiration
        });
    }


    /// <summary>
    /// Logs out a user and revokes the refresh token.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest model)
    {
        await _authService.LogoutAsync(model.RefreshToken);

        return Ok();
    }


    /// <summary>
    /// Logs out a web user and removes the refresh token cookie.
    /// </summary>
    [HttpPost("logout-web")]
    public async Task<IActionResult> LogoutWeb()
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        if (Request.Cookies.TryGetValue(
            "refreshToken",
            out var refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

        return Ok();
    }


    /// <summary>
    /// Sends a password reset email.
    /// </summary>
    /// <remarks>
    /// Returns success even if the email is not registered.
    /// </remarks>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        await _authService.ForgotPasswordAsync(model.Email);

        return Ok();
    }


    /// <summary>
    /// Resets the user's password using a reset token.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        await _authService.ResetPasswordAsync(model);

        return Ok();
    }

    /// <summary>
    /// Generates a CSRF token for cookie-based authentication requests.
    /// </summary>
    [HttpGet("csrf-token")]
    public ActionResult<CsrfResponse> GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new CsrfResponse
        {
            csrfToken = tokens.RequestToken
        });
    }
}