using Application.Abstractions.Services;
using Application.Settings;
using MailKit.Net.Smtp;
using System.IO;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services;

internal class MailKitEmailSender(
    IOptions<EmailSettings> settings,
    RazorLightRenderer templateRenderer)
    : IEmailSender
{
    public async Task SendAsync<TModel>(
        string to,
        string subject,
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        var emailSettings = settings.Value;

        if (string.IsNullOrWhiteSpace(emailSettings.Host))
            throw new InvalidOperationException("Email host is not configured.");

        if (string.IsNullOrWhiteSpace(emailSettings.From))
            throw new InvalidOperationException("Email sender address is not configured.");

        var htmlBody = await templateRenderer.RenderAsync(
            templateFileName,
            model);

        var message = new MimeMessage();

        if (!MailboxAddress.TryParse(emailSettings.From, out var fromAddress))
            throw new InvalidOperationException("Email sender address is not a valid RFC 5321 address.");
        message.From.Add(fromAddress);

        if (!MailboxAddress.TryParse(to, out var toAddress))
            throw new ArgumentException($"Invalid recipient address: '{to}'", nameof(to));
        message.To.Add(toAddress);

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var client = new SmtpClient();

        var secureSocketOptions = emailSettings.UseSslOnConnect
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await client.ConnectAsync(
            emailSettings.Host,
            emailSettings.Port,
            secureSocketOptions,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(emailSettings.UserName))
        {
            await client.AuthenticateAsync(
                emailSettings.UserName,
                emailSettings.Password,
                cancellationToken);
        }

         await client.SendAsync(
            message,
            cancellationToken);

        await client.DisconnectAsync(
            true,
            cancellationToken);
    }
}