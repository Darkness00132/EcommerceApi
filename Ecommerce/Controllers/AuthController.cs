using Application.Features.Auth.ConfirmEmail;
using Application.Features.Auth.Dtos;
using Application.Features.Auth.ForgotPassword;
using Application.Features.Auth.Login;
using Application.Features.Auth.Logout;
using Application.Features.Auth.RefreshToken;
using Application.Features.Auth.Register;
using Application.Features.Auth.ResetPassword;
using Application.Settings;
using Ecommerce.Api.Contracts;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly ISender _sender;
    private readonly JwtSettings _jwtSettings;
    private readonly IAntiforgery _antiforgery;

    public AuthController(
        ISender sender,
        IOptions<JwtSettings> jwtSettings,
        IAntiforgery antiforgery)
    {
        _sender = sender;
        _jwtSettings = jwtSettings.Value;
        _antiforgery = antiforgery;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// Creates the account and starts the email-confirmation process.
    /// The user must confirm their email address before signing in.
    /// </remarks>
    /// <param name="command">
    /// The user registration information.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 201 Created response when the account is created successfully.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);

        // The command doesn't return an ID, so no resource location is supplied.
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <remarks>
    /// Uses the user identifier and confirmation token generated during
    /// registration to confirm the user's email address.
    /// </remarks>
    /// <param name="command">
    /// The user identifier and email-confirmation token.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 204 No Content response when the email is confirmed successfully.
    /// </returns>
    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Authenticates a user and issues authentication tokens.
    /// </summary>
    /// <remarks>
    /// This endpoint is intended for clients that manage both access and
    /// refresh tokens themselves.
    /// </remarks>
    /// <param name="command">
    /// The user's login credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 200 OK response containing the access and refresh tokens.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            command,
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Authenticates a browser-based client.
    /// </summary>
    /// <remarks>
    /// Returns the access token in the response body and stores the refresh
    /// token in a secure, HTTP-only cookie.
    /// </remarks>
    /// <param name="command">
    /// The user's login credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 200 OK response containing the access token and its expiration time.
    /// </returns>
    [HttpPost("login-web")]
    [ProducesResponseType(typeof(WebAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WebAuthResponse>> LoginWeb(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        var response = await _sender.Send(
            command,
            cancellationToken);

        AppendRefreshTokenCookie(response.RefreshToken);

        return Ok(new WebAuthResponse
        {
            AccessToken = response.AccessToken,
            AccessTokenExpiration = response.AccessTokenExpiration
        });
    }

    /// <summary>
    /// Issues new authentication tokens using a refresh token.
    /// </summary>
    /// <remarks>
    /// This endpoint is intended for clients that manage refresh tokens
    /// themselves.
    /// </remarks>
    /// <param name="command">
    /// The refresh token used to issue a new token pair.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 200 OK response containing the newly issued access and refresh tokens.
    /// </returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            command,
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Issues new authentication tokens using the refresh-token cookie.
    /// </summary>
    /// <remarks>
    /// Validates the antiforgery token, reads the existing refresh token from
    /// its HTTP-only cookie, rotates the refresh token, and returns a new
    /// access token.
    /// </remarks>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 200 OK response containing the new access token, or a 401 Unauthorized
    /// response when the refresh-token cookie is missing or invalid.
    /// </returns>
    [HttpPost("refresh-token-web")]
    [ProducesResponseType(typeof(WebAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WebAuthResponse>> RefreshTokenWeb(
        CancellationToken cancellationToken)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        if (!Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken))
        {
            return Unauthorized();
        }

        var command = new RefreshTokenCommand(refreshToken);

        var response = await _sender.Send(
            command,
            cancellationToken);

        AppendRefreshTokenCookie(response.RefreshToken);

        return Ok(new WebAuthResponse
        {
            AccessToken = response.AccessToken,
            AccessTokenExpiration = response.AccessTokenExpiration
        });
    }

    /// <summary>
    /// Revokes a refresh token and signs out the user.
    /// </summary>
    /// <remarks>
    /// This endpoint is intended for clients that manage refresh tokens
    /// themselves.
    /// </remarks>
    /// <param name="command">
    /// The refresh token that should be revoked.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 204 No Content response when logout is completed.
    /// </returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Signs out a browser-based client.
    /// </summary>
    /// <remarks>
    /// Revokes the refresh token when it is available and removes the
    /// refresh-token cookie from the browser.
    ///
    /// The endpoint succeeds even if the cookie is already missing.
    /// </remarks>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 204 No Content response after the refresh-token cookie is removed.
    /// </returns>
    [HttpPost("logout-web")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogoutWeb(
        CancellationToken cancellationToken)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        if (Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken))
        {
            var command = new LogoutCommand(refreshToken);

            await _sender.Send(
                command,
                cancellationToken);
        }

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Starts the password-reset process.
    /// </summary>
    /// <remarks>
    /// If an account exists for the supplied email address, a password-reset
    /// message is sent.
    ///
    /// The endpoint always returns the same successful status to avoid
    /// revealing whether an email address is registered.
    /// </remarks>
    /// <param name="command">
    /// The email address for which the password-reset process is requested.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 202 Accepted response after the request has been processed.
    /// </returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);

        return Accepted();
    }

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    /// <remarks>
    /// Validates the password-reset token and replaces the user's existing
    /// password with the supplied new password.
    /// </remarks>
    /// <param name="command">
    /// The user identifier, password-reset token, and new password.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the operation when the HTTP request is aborted.
    /// </param>
    /// <returns>
    /// A 204 No Content response when the password is reset successfully.
    /// </returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Creates and stores an antiforgery token.
    /// </summary>
    /// <remarks>
    /// Browser-based clients should include the returned token with requests
    /// that use the refresh-token cookie.
    /// </remarks>
    /// <returns>
    /// A 200 OK response containing the antiforgery request token.
    /// </returns>
    [HttpGet("csrf-token")]
    [ProducesResponseType(typeof(CsrfResponse), StatusCodes.Status200OK)]
    public ActionResult<CsrfResponse> GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new CsrfResponse
        {
            CsrfToken = tokens.RequestToken
        });
    }

    /// <summary>
    /// Creates or replaces the browser's refresh-token cookie.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token to store in the cookie.
    /// </param>
    private void AppendRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            CreateRefreshTokenCookieOptions());
    }

    /// <summary>
    /// Removes the browser's refresh-token cookie.
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            CreateRefreshTokenCookieOptions(
                includeExpiration: false));
    }

    /// <summary>
    /// Creates the shared cookie options used when adding or deleting the
    /// refresh-token cookie.
    /// </summary>
    private CookieOptions CreateRefreshTokenCookieOptions(
        bool includeExpiration = true)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",

            Expires = includeExpiration
                ? DateTimeOffset.UtcNow.AddDays(
                    _jwtSettings.RefreshTokenExpirationDays)
                : null
        };
    }
}