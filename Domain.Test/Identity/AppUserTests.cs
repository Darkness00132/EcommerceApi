using Domain.Entities.Identity;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Test.Identity;

public class AppUserTests
{
    [Fact]
    public void User_Are_Created_When_Valid_Data_Is_Provided()
    {
        // Arrange & Act
        var user = CreateValidUser();

        // Assert
        user.Should().NotBeNull();
        user.FullName.FirstName.Should().Be("mohamed");
        user.FullName.LastName.Should().Be("ahmed");
        user.Email.Should().Be("email@gmail.com");
    }

    [Theory]
    [InlineData("", "ahmed", "email@gmail.com")]
    [InlineData("mohamed", "", "email@gmail.com")]
    [InlineData("mohamed", "ahmed", "")]
    public void User_Are_Not_Created_When_Invalid_Data_Is_Provided(string firstname,string lastname,string email)
    {
        // Arrange & Act
        var user = () => CreateUser(firstname, lastname, email);

        // Assert
        user.Should().Throw<DomainException>();
    }

    [Fact]
    public void User_Can_Add_Refresh_Token_When_Valid_Data_Is_Provided()
    {
        // Arrange
        var user = CreateValidUser();

        //Act
        var refreshToken = user.AddRefreshToken("valid_refresh_token", DateTime.UtcNow.AddHours(1));

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Token.Should().Be("valid_refresh_token");
        user.RefreshTokens.Should().Contain(refreshToken);
        user.RefreshTokens.Should().HaveCount(1);
    }

    [Theory]
    [MemberData(nameof(InvalidTokenData))]
    public void User_Can_Not_Add_Refresh_Token_When_InValid_Data_Is_Provided(string tokenValue,DateTime expiresAt)
    {
        // Arrange
        var user = CreateValidUser();

        //Act
        var refreshToken = () => user.AddRefreshToken(tokenValue, expiresAt);

        // Assert
        refreshToken.Should().Throw<DomainException>();
    }

    [Fact]
    public void User_Can_Revoke_Refresh_Token_When_Valid_Token_Is_Provided()
    {
        // Arrange
        var user = CreateValidUser();
        var refreshToken = user.AddRefreshToken("valid_refresh_token", DateTime.UtcNow.AddHours(1));

        //Act
        user.RevokeRefreshToken(refreshToken);

        // Assert
        user.RefreshTokens.Should().NotContain(refreshToken);
    }

    private AppUser CreateUser(string firstName, string lastName, string email)
    {
        return new AppUser(new FullName(firstName, lastName),email);
    }

    private AppUser CreateValidUser()
    {
        return CreateUser("mohamed", "ahmed", "email@gmail.com");
    }

    public static IEnumerable<object[]> InvalidTokenData =>
    new List<object[]>
    {
            new object[] { "", DateTime.UtcNow.AddDays(1) },
            new object[] { "valid-token", DateTime.UtcNow.AddDays(-1) },
            new object[] { "valid-token", DateTime.UtcNow },
    };
}
