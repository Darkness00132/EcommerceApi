using Application.Features.Account.Commands.ConfirmEmail;
using Application.Features.Account.Commands.ForgotPassword;
using Application.Features.Account.Commands.Login;
using Application.Features.Account.Commands.Refresh;
using Application.Features.Account.Commands.Register;
using Application.Features.Account.Commands.ResetPassword;
using Application.Features.Account.Commands.RevokeToken;
using Application.Features.Account.Dto;
using Application.Features.Account.Queries.GetCurrent;
using Ecommerce.Api.Constants;
using Ecommerce.Api.Contracts.Account;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// Provides endpoints for account registration, authentication,
/// token management, email confirmation, and password recovery.
/// </summary>
[ApiController]
[Route("api/account")]
public sealed class AccountController(
    ISender sender,
    IAntiforgery antiforgery) : ControllerBase
{
    /// <summary>
    /// Registers a new account and initiates email confirmation.
    /// </summary>
    /// <response code="201">The account was created successfully.</response>
    /// <response code="400">
    /// The supplied registration data is invalid.
    /// </response>
    /// <response code="409">
    /// An account with the supplied email address already exists.
    /// </response>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        RegisterAccountCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return Created();
    }

    /// <summary>
    /// Confirms a user's email address using a confirmation token.
    /// </summary>
    /// <response code="204">The email address was confirmed successfully.</response>
    /// <response code="400">
    /// The confirmation token is invalid or the request data is invalid.
    /// </response>
    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Authenticates a user and returns an access and refresh token pair.
    /// </summary>
    /// <response code="200">Authentication was successful.</response>
    /// <response code="400">The supplied credentials are invalid.</response>
    /// <response code="401">The email or password is incorrect.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccountTokenDto>> Login(
        LoginAccountCommand command,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Exchanges a refresh token for a new access and refresh token pair.
    /// </summary>
    /// <response code="200">The token pair was refreshed successfully.</response>
    /// <response code="400">The refresh token is invalid.</response>
    /// <response code="401">The refresh token is expired or revoked.</response>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AccountTokenDto>> Refresh(
        RefreshAccountTokenCommand command,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Revokes the supplied refresh token and signs the user out.
    /// </summary>
    /// <response code="204">The refresh token was revoked successfully.</response>
    /// <response code="400">The supplied token is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        RevokeAccountTokenCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Initiates the password recovery process for an account.
    /// </summary>
    /// <remarks>
    /// This endpoint always returns a successful response, regardless of
    /// whether an account exists for the supplied email address, to prevent
    /// account enumeration.
    /// </remarks>
    /// <response code="202">
    /// The password recovery request was accepted.
    /// </response>
    /// <response code="400">The supplied email address is invalid.</response>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return Accepted();
    }

    /// <summary>
    /// Resets an account password using a valid password reset token.
    /// </summary>
    /// <response code="204">The password was reset successfully.</response>
    /// <response code="400">
    /// The reset token is invalid or the new password does not meet the requirements.
    /// </response>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets the account information of the currently authenticated user.
    /// </summary>
    /// <response code="200">The account information was retrieved successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentAccountDto>> Me(
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetCurrentAccountQuery(),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Generates an anti-forgery token for browser-based authentication.
    /// </summary>
    /// <response code="200">The anti-forgery token was generated successfully.</response>
    [AllowAnonymous]
    [HttpGet("csrf-web")]
    public ActionResult<CsrfTokenResponse> Csrf()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new CsrfTokenResponse(tokens.RequestToken!));
    }

    /// <summary>
    /// Authenticates a browser client and stores the refresh token
    /// in a secure, HttpOnly cookie.
    /// </summary>
    /// <response code="200">
    /// Authentication was successful and an access token was returned.
    /// </response>
    /// <response code="400">The supplied credentials are invalid.</response>
    /// <response code="401">The email or password is incorrect.</response>
    [AllowAnonymous]
    [HttpPost("login-web")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<WebAccountResponse>> LoginWeb(
        LoginAccountCommand command,
        CancellationToken cancellationToken)
    {
        var tokens = await sender.Send(command, cancellationToken);

        SetRefreshTokenCookie(tokens);

        return Ok(new WebAccountResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt));
    }

    /// <summary>
    /// Refreshes the current browser session using the refresh token
    /// stored in the authentication cookie.
    /// </summary>
    /// <response code="200">The browser session was refreshed successfully.</response>
    /// <response code="401">
    /// The refresh token cookie is missing, invalid, expired, or revoked.
    /// </response>
    [AllowAnonymous]
    [HttpPost("refresh-web")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<WebAccountResponse>> RefreshWeb(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                AccountApiConstants.RefreshTokenCookieName,
                out var refreshToken)) {
            return Unauthorized();
        }

        var tokens = await sender.Send(
            new RefreshAccountTokenCommand(refreshToken),
            cancellationToken);

        SetRefreshTokenCookie(tokens);

        return Ok(new WebAccountResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt));
    }

    /// <summary>
    /// Revokes the browser refresh token and removes the authentication cookie.
    /// </summary>
    /// <response code="204">
    /// The browser session was revoked successfully.
    /// </response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("revoke-web")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeWeb(
        CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(
                AccountApiConstants.RefreshTokenCookieName,
                out var refreshToken)) {
            await sender.Send(
                new RevokeAccountTokenCommand(refreshToken),
                cancellationToken);
        }

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    private void SetRefreshTokenCookie(AccountTokenDto tokens)
    {
        var cookieOptions = new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = tokens.RefreshTokenExpiresAt
        };

        Response.Cookies.Append(
            AccountApiConstants.RefreshTokenCookieName,
            tokens.RefreshToken,
            cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            AccountApiConstants.RefreshTokenCookieName);
    }
}
