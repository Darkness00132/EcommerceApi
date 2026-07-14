using Ecommerce.Data.Entities;
using Ecommerce.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ecommerce.ThirdPartyServices;

public class EmailSender : IEmailSender<AppUser>
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<EmailSettings> options,
        ILogger<EmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendConfirmationLinkAsync(
        AppUser user,
        string email,
        string confirmationLink)
    {
        var body = $"""
            <h2>Welcome {user.FirstName + " " + user.LastName}!</h2>

            <p>Thank you for creating an account.</p>

            <p>Please confirm your email by clicking the link below:</p>

            <p>
                <a href="{confirmationLink}">
                    Confirm Email
                </a>
            </p>

            <p>If you didn't create this account, you can safely ignore this email.</p>
            """;

        await SendEmailAsync(
            email,
            "Confirm your email",
            body);
    }

    public Task SendPasswordResetCodeAsync(
        AppUser user,
        string email,
        string resetCode)
    {
        throw new NotSupportedException(
            "Password reset codes are not supported.");
    }

    public async Task SendPasswordResetLinkAsync(
        AppUser user,
        string email,
        string resetLink)
    {
        var body = $"""
            <h2>Hello {user.FirstName + " " + user.LastName},</h2>

            <p>We received a request to reset your password.</p>

            <p>
                <a href="{resetLink}">
                    Reset Password
                </a>
            </p>

            <p>If you didn't request a password reset, you can safely ignore this email.</p>
            """;

        await SendEmailAsync(
            email,
            "Reset your password",
            body);
    }

    private async Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.DisplayName,
                _settings.Email));

        message.To.Add(
            MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _settings.Email,
                _settings.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Email}. Subject: {Subject}",
                to,
                subject);

            throw;
        }
    }
}