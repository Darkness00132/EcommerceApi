using System.Linq.Expressions;
using Application.Abstractions.Services;
using Application.Exceptions;
using Application.Features.Account.Commands.Register;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Application.Test.FeaturesHandlers;

public class RegisterAccountCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IBackgroundJobService> _backgroundJobsMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly RegisterAccountCommandHandler _sut;

    public RegisterAccountCommandHandlerTests()
    {
        _userManagerMock = CreateMockUserManager();
        _backgroundJobsMock = new Mock<IBackgroundJobService>();
        _configurationMock = new Mock<IConfiguration>();

        _sut = new RegisterAccountCommandHandler(
            _userManagerMock.Object,
            _backgroundJobsMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterAccountCommand("John", "Doe", "existing@example.com", "Password123!");
        var existingUser = CreateUser(command.Email);

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _sut.Handle(command, CancellationToken.None));

        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
        _backgroundJobsMock.Verify(b => b.Enqueue<IEmailSender>(It.IsAny<Expression<Func<IEmailSender, Task>>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenUserCreationFails()
    {
        // Arrange
        var command = new RegisterAccountCommand("John", "Doe", "new@example.com", "weak");

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser?)null);

        var identityErrors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password must be at least 6 characters." },
            new IdentityError { Code = "PasswordRequiresDigit", Description = "Passwords must have at least one digit ('0'-'9')." }
        };

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _sut.Handle(command, CancellationToken.None));

        Assert.NotNull(exception);
        _userManagerMock.Verify(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<AppUser>()), Times.Never);
        _backgroundJobsMock.Verify(b => b.Enqueue<IEmailSender>(It.IsAny<Expression<Func<IEmailSender, Task>>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenFrontendUrlIsMissing()
    {
        // Arrange
        var command = new RegisterAccountCommand("John", "Doe", "new@example.com", "Password123!");

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync("token-123");

        _configurationMock
            .Setup(c => c["FrontendUrl"])
            .Returns((string?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(command, CancellationToken.None));

        Assert.Equal("FrontendUrl configuration is required.", exception.Message);
        _backgroundJobsMock.Verify(b => b.Enqueue<IEmailSender>(It.IsAny<Expression<Func<IEmailSender, Task>>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndEnqueueEmail_WhenRequestIsValid()
    {
        // Arrange
        var command = new RegisterAccountCommand("John", "Doe", "john.doe@example.com", "Password123!");
        var rawToken = "raw/confirmation+token==";

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.Is<AppUser>(u => u.Email == command.Email), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.Is<AppUser>(u => u.Email == command.Email)))
            .ReturnsAsync(rawToken);

        _configurationMock
            .Setup(c => c["FrontendUrl"])
            .Returns("https://myapp.com/");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(m => m.CreateAsync(It.Is<AppUser>(u => u.Email == command.Email), command.Password), Times.Once);
        _userManagerMock.Verify(m => m.GenerateEmailConfirmationTokenAsync(It.Is<AppUser>(u => u.Email == command.Email)), Times.Once);
        _backgroundJobsMock.Verify(
            b => b.Enqueue<IEmailSender>(
                It.IsAny<Expression<Func<IEmailSender, Task>>>(),
                It.IsAny<string>()),
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
