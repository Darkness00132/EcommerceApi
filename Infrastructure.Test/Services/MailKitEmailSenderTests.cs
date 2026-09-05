using Application.Features.Account.Dto;
using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.Test.Services;

public class MailKitEmailSenderTests
{
    private readonly IOptions<EmailSettings> _emailSettings;
    private readonly RazorLightRenderer _render;
    private readonly Mock<ISmtpClient> _smtpClient;
    private readonly MailKitEmailSender _sut;
    public MailKitEmailSenderTests()
    {
        _emailSettings = Options.Create(new EmailSettings {
            From = "test@example.com",
            Host = "smtp.example.com",
            Password = "password",
            Port = 587,
            UseSslOnConnect = false
        });
        _render = new RazorLightRenderer();
        _smtpClient = new Mock<ISmtpClient>();
        _sut = new MailKitEmailSender(_emailSettings, _render, _smtpClient.Object);
    }

    [Fact]
    public async Task Confirmation_Email_Should_Be_Send()
    {
        // Assert
        var confirmationEmail = new EmailConfirmationEmailModel("test user", "confirmation-link");

        // Act
        var act = () => _sut.SendAsync<EmailConfirmationEmailModel>("user@example.com","confirm-email","ConfirmEmail",confirmationEmail);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
