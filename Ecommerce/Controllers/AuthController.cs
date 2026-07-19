using Ecommerce.DTOs.Auth;
using Ecommerce.Services;
using Ecommerce.Settings;
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
    /// The account is created with a customer role and remains inactive
    /// until the user confirms their email address through the link sent by email.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        await _authService.RegisterAsync(model);

        return Ok();
    }


    /// <summary>
    /// Confirms a user's email address.
    /// The user must provide the user identifier and confirmation token
    /// received from the registration email before they can authenticate.
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
    /// Authenticates a user and returns JWT tokens.
    /// This endpoint is designed for clients that store and manage tokens themselves,
    /// such as mobile applications.
    /// The client must send the refresh token when requesting a new access token.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest model)
    {
        var response = await _authService.LoginAsync(model);

        return Ok(response);
    }


    /// <summary>
    /// Authenticates a web user and creates a refresh token cookie.
    /// The refresh token is stored in an HttpOnly cookie so it cannot be accessed
    /// by JavaScript, reducing the impact of XSS attacks.
    /// 
    /// The returned access token should be stored by the frontend and sent
    /// in the Authorization header for protected API requests.
    /// </summary>
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
    /// Refreshes an expired access token using a refresh token provided by the client.
    /// 
    /// This endpoint is intended for mobile applications or clients that manage
    /// refresh tokens manually instead of storing them in browser cookies.
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        RefreshTokenRequest model)
    {
        var response =
            await _authService.RefreshTokenAsync(model.RefreshToken);

        return Ok(response);
    }


    /// <summary>
    /// Refreshes an expired access token using the refresh token stored
    /// inside the browser's HttpOnly cookie.
    /// 
    /// This endpoint is intended for web applications.
    /// The browser automatically sends the refresh token cookie with the request.
    /// </summary>
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
    /// Logs out a client by revoking the provided refresh token.
    /// 
    /// Intended for mobile applications where the refresh token
    /// is stored and sent manually by the client.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest model)
    {
        await _authService.LogoutAsync(model.RefreshToken);

        return Ok();
    }


    /// <summary>
    /// Logs out a web user by revoking the refresh token stored in the browser cookie
    /// and removing the cookie from the client.
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
    /// 
    /// The endpoint always returns a successful response even when the email
    /// does not exist to prevent attackers from discovering registered accounts.
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        await _authService.ForgotPasswordAsync(model.Email);

        return Ok();
    }


    /// <summary>
    /// Changes the user's password using the reset token received through email.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        await _authService.ResetPasswordAsync(model);

        return Ok();
    }

    /// <summary>
    /// Generates a CSRF token required for cookie-based requests.
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