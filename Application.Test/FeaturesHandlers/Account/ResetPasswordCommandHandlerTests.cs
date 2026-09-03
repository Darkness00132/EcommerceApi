using System.Linq.Expressions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Features.Account.Commands.ResetPassword;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Test.FeaturesHandlers.Account;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly Mock<IRepository<RefreshToken>> _refreshTokens;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ResetPasswordCommand _command;
    private readonly ResetPasswordCommandHandler _sut;

    public ResetPasswordCommandHandlerTests()
    {
        _userManager = TestMocks.CreateMockUserManager();
        _refreshTokens = new Mock<IRepository<RefreshToken>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _command = new ResetPasswordCommand(
            "test@gmail.com",
            "reset-token",
            "NewPassword123!");

        _sut = new ResetPasswordCommandHandler(
            _userManager.Object,
            _refreshTokens.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task A_User_Can_Reset_Their_Password()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManager
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.ResetPasswordAsync(
                user,
                _command.Token,
                _command.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        _refreshTokens
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = () => _sut.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task A_Password_Reset_Is_Rejected_When_The_User_Does_Not_Exist()
    {
        // Arrange
        _userManager
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync((AppUser?)null);

        // Act
        var act = () => _sut.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task A_Password_Reset_Is_Rejected_When_The_Reset_Request_Is_Invalid()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManager
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.ResetPasswordAsync(
                user,
                _command.Token,
                _command.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Code = "InvalidToken" }));

        // Act
        var act = () => _sut.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Active_Sessions_Are_Ended_After_A_Password_Reset()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        var refreshToken1 = user.AddRefreshToken(
            "token-1",
            DateTime.UtcNow.AddDays(7));

        var refreshToken2 = user.AddRefreshToken(
            "token-2",
            DateTime.UtcNow.AddDays(7));

        _userManager
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.ResetPasswordAsync(
                user,
                _command.Token,
                _command.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        _refreshTokens
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([refreshToken1, refreshToken2]);

        // Act
        await _sut.Handle(_command, CancellationToken.None);

        // Assert
        refreshToken1.RevokedAt.Should().NotBeNull();
        refreshToken2.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task A_Password_Reset_Can_Succeed_When_There_Are_No_Active_Sessions()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManager
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.ResetPasswordAsync(
                user,
                _command.Token,
                _command.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        _refreshTokens
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = () => _sut.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    private static AppUser CreateUser(string email)
        => new AppUser(new FullName("John", "Doe"),email);
}
