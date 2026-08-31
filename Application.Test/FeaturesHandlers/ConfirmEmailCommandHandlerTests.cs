using Application.Exceptions;
using Application.Features.Account.Commands.ConfirmEmail;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Application.Test.FeaturesHandlers;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly ConfirmEmailCommandHandler _sut;

    public ConfirmEmailCommandHandlerTests()
    {
        _userManagerMock = CreateMockUserManager();
        _sut = new ConfirmEmailCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenUserExistsAndTokenIsValid()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "valid-token");
        var user = CreateUser(command.Email);

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.ConfirmEmailAsync(user, command.Token))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, command.Token), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
    {
        // Arrange
        var command = new ConfirmEmailCommand("missing@example.com", "any-token");

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.Handle(command, CancellationToken.None));

        Assert.Equal("The email confirmation request is invalid.", exception.Message);

        _userManagerMock.Verify(
            m => m.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenConfirmEmailFails()
    {
        // Arrange
        var command = new ConfirmEmailCommand("user@example.com", "invalid-token");
        var user = CreateUser(command.Email);
        var identityError = new IdentityError {
            Code = "InvalidToken",
            Description = "Token is invalid or expired."
        };

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.ConfirmEmailAsync(user, command.Token))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _sut.Handle(command, CancellationToken.None));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("InvalidToken"));
        Assert.Contains("Token is invalid or expired.", exception.Errors["InvalidToken"]);
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
