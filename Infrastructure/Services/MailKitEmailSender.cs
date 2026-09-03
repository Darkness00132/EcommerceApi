using Application.Abstractions.Services;
using Infrastructure.Abstractions;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services;

internal class MailKitEmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ISmtpClient _smtpClient;

    public MailKitEmailSender(
        IOptions<EmailSettings> settings,
        IEmailTemplateRenderer templateRenderer,
        ISmtpClient smtpClient)
    {
        _emailSettings = settings.Value;
        _templateRenderer = templateRenderer;
        _smtpClient = smtpClient;
    }

    public async Task SendAsync<TModel>(
        string to,
        string subject,
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken = default)
    {

        if (!MailboxAddress.TryParse(
                _emailSettings.From,
                out var fromAddress)) {
            throw new InvalidOperationException(
                "Email sender address is not valid.");
        }

        var htmlBody = await _templateRenderer.RenderAsync(
            templateFileName,
            model);

        var message = new MimeMessage();

        message.From.Add(fromAddress);
        message.To.Add(new MailboxAddress(null, to));
        message.Subject = subject;

        message.Body = new BodyBuilder {
            HtmlBody = htmlBody
        }.ToMessageBody();

        var secureSocketOptions = _emailSettings.UseSslOnConnect
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await _smtpClient.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            secureSocketOptions,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_emailSettings.UserName)) {
            await _smtpClient.AuthenticateAsync(
                _emailSettings.UserName,
                _emailSettings.Password,
                cancellationToken);
        }

        await _smtpClient.SendAsync(
            message,
            cancellationToken);

        await _smtpClient.DisconnectAsync(
            true,
            cancellationToken);
    }
}
