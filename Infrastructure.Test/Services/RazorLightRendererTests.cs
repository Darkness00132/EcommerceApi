using Application.Features.Account.Dto;
using FluentAssertions;
using Infrastructure.Services;

namespace Infrastructure.Test.Services;

public class RazorLightRendererTests
{
    private readonly RazorLightRenderer _sut;
    public RazorLightRendererTests()
    {
        _sut = new RazorLightRenderer();
    }

    [Theory]
    [MemberData(nameof(GetExistedTemplates))]
    public async Task Convert_Razor_Page_To_Html_When_Provide_Existed_Template(
        string templateName, object model)
    {
        // Arrange & Act
        var result = await _sut.RenderAsync(templateName, model);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        // Check if the result contains HTML tags
        result.Should().MatchRegex(@"<[^>]+>");
    }

    public static IEnumerable<object[]> GetExistedTemplates()
    {
        return [
            new object[] { "ConfirmEmail"
            , new EmailConfirmationEmailModel("test user", "confirmation-link") },

            new object[] { "ForgotPassword"
            , new ForgotPasswordEmailModel("test user", "confirmation-link") },
            ];
    }
}
