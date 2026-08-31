using Application.Features.Account.Dto;
using Infrastructure.Settings;
using Infrastructure.Abstractions;
using Infrastructure.Services;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;

namespace Infrastructure.Test.services;

public class MailKitEmailSenderTests
{
    private const string Recipient = "recipient@example.com";
    private const string Subject = "Confirm your email";
    private const string Template = "ConfirmEmail";
    private const string HtmlBody = "<h1>Test email body</h1>";

    private readonly EmailSettings _emailSettings = CreateSettings();

    private readonly Mock<IEmailTemplateRenderer> _templateRenderer = new();
    private readonly Mock<ISmtpClient> _smtpClient = new();

    public MailKitEmailSenderTests()
    {
        _templateRenderer
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(HtmlBody);
    }

    [Fact]
    public async Task SendAsync_ShouldRenderAndSendEmail()
    {
        // Arrange
        var model = new EmailConfirmationEmailModel(
            "John Doe",
            "https://example.com/confirm");

        var sender = CreateSender();

        // Act
        await sender.SendAsync(
            Recipient,
            Subject,
            Template,
            model);

        // Assert
        _templateRenderer.Verify(
            x => x.RenderAsync(Template, model),
            Times.Once);

        _smtpClient.Verify(
            x => x.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _smtpClient.Verify(
            x => x.AuthenticateAsync(
                _emailSettings.UserName,
                _emailSettings.Password,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _smtpClient.Verify(
            x => x.SendAsync(
                It.Is<MimeMessage>(message =>
                    message.Subject == Subject &&
                    message.From.Mailboxes.Single().Address
                        == _emailSettings.From &&
                    message.To.Mailboxes.Single().Address
                        == Recipient &&
                    message.HtmlBody == HtmlBody),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress?>()),
            Times.Once);

        _smtpClient.Verify(
            x => x.DisconnectAsync(
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(false, 587, SecureSocketOptions.StartTls)]
    [InlineData(true, 465, SecureSocketOptions.SslOnConnect)]
    public async Task SendAsync_ShouldUseCorrectSecurityOption(
        bool useSslOnConnect,
        int port,
        SecureSocketOptions expectedOption)
    {
        // Arrange
        var settings = CreateSettings(
            port: port,
            useSslOnConnect: useSslOnConnect);

        var sender = CreateSender(settings);

        // Act
        await sender.SendAsync(
            Recipient,
            "Test",
            Template,
            new object());

        // Assert
        _smtpClient.Verify(
            x => x.ConnectAsync(
                settings.Host,
                settings.Port,
                expectedOption,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private MailKitEmailSender CreateSender(
        EmailSettings? settings = null)
    {
        return new MailKitEmailSender(
            Options.Create(settings ?? _emailSettings),
            _templateRenderer.Object,
            _smtpClient.Object);
    }

    private static EmailSettings CreateSettings(
        string host = "smtp.example.com",
        int port = 587,
        bool useSslOnConnect = false,
        string userName = "test-user",
        string password = "test-password",
        string from = "sender@example.com")
    {
        return new EmailSettings {
            Host = host,
            Port = port,
            UseSslOnConnect = useSslOnConnect,
            UserName = userName,
            Password = password,
            From = from
        };
    }
}
