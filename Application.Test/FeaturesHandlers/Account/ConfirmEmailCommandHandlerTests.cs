using Application.Exceptions;
using Application.Features.Account.Commands.ConfirmEmail;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Test.FeaturesHandlers.Account;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly ConfirmEmailCommand _command;
    private readonly ConfirmEmailCommandHandler _sut;

    public ConfirmEmailCommandHandlerTests()
    {
        _userManagerMock = TestMocks.CreateMockUserManager();
        _command = new ConfirmEmailCommand("user@example.com", "invalid-token");

        _sut = new ConfirmEmailCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task A_User_Can_Confirm_Their_Email()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.ConfirmEmailAsync(user, _command.Token))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var act = () => _sut.Handle(
            _command,
            CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task An_Email_Confirmation_Request_Is_Rejected_When_The_User_Does_Not_Exist()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync((AppUser?)null);

        // Act
        var act = () => _sut.Handle(
            _command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task An_Email_Confirmation_Request_Is_Rejected_When_The_Confirmation_Is_Invalid()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.ConfirmEmailAsync(user, _command.Token))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var act = () => _sut.Handle(
            _command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    private static AppUser CreateUser(string email)
    {
        return new AppUser(new FullName("John", "Doe"), email);
    }
}
