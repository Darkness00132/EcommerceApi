using Application.Interfaces.Services;
using Infrastructure.Services;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly RazorService _razorService;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger,
        RazorService razorService)
    {
        _settings = settings.Value;
        _logger = logger;
        _razorService = razorService;
    }


    public async Task SendAsync<TModel>(
        string to,
        string subject,
        string templateName,
        TModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        var htmlBody = await _razorService.GetHtmlAsync(
            templateName,
            model);

        var message = CreateMessage(
            to,
            subject,
            htmlBody);

        await SendEmailAsync(message);
    }


    private MimeMessage CreateMessage(
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

        return message;
    }


    private async Task SendEmailAsync(MimeMessage message)
    {
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
                "Failed to send email.");

            throw;
        }
        finally
        {
            message.Dispose();
        }
    }
}