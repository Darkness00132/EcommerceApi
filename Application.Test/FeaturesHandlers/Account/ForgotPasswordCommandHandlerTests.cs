using Application.Abstractions.Services;
using Application.Features.Account.Commands.ForgotPassword;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.Test.FeaturesHandlers.Account;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IBackgroundJobService> _backgroundJobsMock;
    private readonly IConfiguration _configuration;
    private readonly ForgotPasswordCommandHandler _sut;

    private readonly ForgotPasswordCommand _command = new ForgotPasswordCommand("user@example.com");

    public ForgotPasswordCommandHandlerTests()
    {
        _userManagerMock = TestMocks.CreateMockUserManager();
        _backgroundJobsMock = new Mock<IBackgroundJobService>();

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["FrontendUrl"] = "https://example.com"
            })
            .Build();

        _sut = new ForgotPasswordCommandHandler(
            _userManagerMock.Object,
            _backgroundJobsMock.Object,
            _configuration);
    }

    [Fact]
    public async Task A_Password_Reset_Request_Can_Be_Created()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token");

        // Act
        var act = () => _sut.Handle(
            _command,
            CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task A_Password_Reset_Request_Is_Ignored_When_The_User_Does_Not_Exist()
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
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task A_Password_Reset_Request_Is_Rejected_When_The_Frontend_Url_Is_Missing()
    {
        // Arrange
        var user = CreateUser(_command.Email);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        var sut = new ForgotPasswordCommandHandler(
            _userManagerMock.Object,
            _backgroundJobsMock.Object,
            configuration);

        // Act
        var act = () => sut.Handle(
            _command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private static AppUser CreateUser(string email)
    {
        return new AppUser(new FullName("John", "Doe"), email);
    }
}
