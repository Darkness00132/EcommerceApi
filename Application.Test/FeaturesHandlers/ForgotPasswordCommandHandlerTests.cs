using System.Linq.Expressions;
using Application.Abstractions.Services;
using Application.Constants;
using Application.Features.Account.Commands.ForgotPassword;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.Test.FeaturesHandlers;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IBackgroundJobService> _backgroundJobsMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ForgotPasswordCommandHandler _sut;

    public ForgotPasswordCommandHandlerTests()
    {
        _userManagerMock = CreateMockUserManager();
        _backgroundJobsMock = new Mock<IBackgroundJobService>();
        _configurationMock = new Mock<IConfiguration>();

        _sut = new ForgotPasswordCommandHandler(
            _userManagerMock.Object,
            _backgroundJobsMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEarly_WhenUserNotFound()
    {
        // Arrange
        var command = new ForgotPasswordCommand("nonexistent@example.com");

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser?)null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.GeneratePasswordResetTokenAsync(It.IsAny<AppUser>()), Times.Never);
        _backgroundJobsMock.Verify(
            b => b.Enqueue<IEmailSender>(
                It.IsAny<Expression<Func<IEmailSender, Task>>>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenFrontendUrlConfigIsMissing()
    {
        // Arrange
        var command = new ForgotPasswordCommand("user@example.com");
        var user = CreateUser(command.Email);

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token-123");

        _configurationMock
            .Setup(c => c["FrontendUrl"])
            .Returns((string?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.Handle(command, CancellationToken.None));

        Assert.Equal("FrontendUrl configuration is required.", exception.Message);
        _backgroundJobsMock.Verify(
            b => b.Enqueue<IEmailSender>(
                It.IsAny<Expression<Func<IEmailSender, Task>>>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldGenerateTokenAndEnqueueEmailWithCriticalPriority_WhenUserExistsAndConfigIsValid()
    {
        // Arrange
        var command = new ForgotPasswordCommand("user@example.com");
        var user = CreateUser(command.Email);
        var rawToken = "raw/token+123==";

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync(rawToken);

        _configurationMock
            .Setup(c => c["FrontendUrl"])
            .Returns("https://myapp.com/");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(m => m.GeneratePasswordResetTokenAsync(user), Times.Once);

        // Verifies both that the job was enqueued AND that it used Critical priority
        _backgroundJobsMock.Verify(
            b => b.Enqueue<IEmailSender>(
                It.IsAny<Expression<Func<IEmailSender, Task>>>(),
                BackgroundJobQueuesPriority.Critical),
            Times.Once);
    }

    private static AppUser CreateUser(string email)
    {
        return new AppUser(new FullName("John", "Doe"), email);
    }

    private static Mock<UserManager<AppUser>> CreateMockUserManager()
    {
        var storeMock = new Mock<IUserStore<AppUser>>();
        return new Mock<UserManager<AppUser>>(
            storeMock.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
