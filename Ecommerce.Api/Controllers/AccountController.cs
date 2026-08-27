using Application.Features.Account.Commands.ConfirmEmail;
using Application.Features.Account.Commands.ForgotPassword;
using Application.Features.Account.Commands.Login;
using Application.Features.Account.Commands.RefreshToken;
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
/// Provides account management endpoints including registration,
/// authentication, token management, email confirmation, and password recovery.
/// </summary>
[ApiController]
[Route("api/account")]
public sealed class AccountController(
    ISender sender,
    IAntiforgery antiforgery) : ControllerBase
{
    /// <summary>
    /// Creates a new account and sends an email confirmation message.
    /// </summary>
    /// <param name="command">The registration details.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>A successful response when the account is created.</returns>
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
    /// Confirms an account email address using the provided confirmation token.
    /// </summary>
    /// <param name="command">The email confirmation request.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>No content when the email address is successfully confirmed.</returns>
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
    /// Authenticates a client and returns access and refresh tokens.
    /// </summary>
    /// <param name="command">The account credentials.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>The generated access and refresh tokens.</returns>
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
    /// Exchanges a refresh token for a new access token and refresh token pair.
    /// </summary>
    /// <param name="command">The refresh token request.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>A newly generated token pair.</returns>
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
    /// Revokes a refresh token and signs the user out.
    /// </summary>
    /// <param name="command">The token revocation request.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>No content when the token is successfully revoked.</returns>
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
    /// Sends a password reset email if the account exists.
    /// </summary>
    /// <remarks>
    /// This endpoint always returns success to avoid revealing
    /// whether an account is registered with the supplied email address.
    /// </remarks>
    /// <param name="command">The password reset request.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>An accepted response.</returns>
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
    /// <param name="command">The password reset details.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>No content when the password is successfully reset.</returns>
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
    /// Returns details about the currently authenticated account.
    /// </summary>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>The current account information.</returns>
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
    /// Generates an anti-forgery token for browser-based authentication flows.
    /// </summary>
    /// <returns>The generated anti-forgery request token.</returns>
    [AllowAnonymous]
    [HttpGet("csrf-web")]
    public ActionResult<CsrfTokenResponse> Csrf()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new CsrfTokenResponse(tokens.RequestToken!));
    }

    /// <summary>
    /// Authenticates a browser client and stores the refresh token
    /// in a secure HttpOnly cookie.
    /// </summary>
    /// <param name="command">The account credentials.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>The access token information for the browser client.</returns>
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
    /// Refreshes the current browser session using the refresh-token cookie.
    /// </summary>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>A new access token.</returns>
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
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>No content when the session is successfully revoked.</returns>
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

    /// <summary>
    /// Stores the refresh token in a secure HttpOnly cookie.
    /// </summary>
    /// <param name="tokens">The generated account tokens.</param>
    private void SetRefreshTokenCookie(AccountTokenDto tokens)
    {
        var cookieOptions = new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokens.RefreshTokenExpiresAt
        };

        Response.Cookies.Append(
            AccountApiConstants.RefreshTokenCookieName,
            tokens.RefreshToken,
            cookieOptions);
    }

    /// <summary>
    /// Removes the refresh-token cookie from the current response.
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            AccountApiConstants.RefreshTokenCookieName);
    }
}
