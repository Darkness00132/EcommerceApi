using Application.Features.Account.Dto;
using Infrastructure.Services;

namespace Infrastructure.Test.services;

public class RazorLightRendererTests
{
    [Fact]
    public void ShouldContainConfirmEmailTemplate()
    {
        var resources = typeof(RazorLightRenderer)
            .Assembly
            .GetManifestResourceNames();

        Assert.Contains(
            "Infrastructure.EmailTemplates.ConfirmEmail.cshtml",
            resources);
    }

    [Fact]
    public void ShouldContainForgotPasswordTemplate()
    {
        var resources = typeof(RazorLightRenderer)
            .Assembly
            .GetManifestResourceNames();

        Assert.Contains(
            "Infrastructure.EmailTemplates.ForgotPassword.cshtml",
            resources);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderTemplateWithModel()
    {
        // Arrange
        var renderer = new RazorLightRenderer();

        var model = new EmailConfirmationEmailModel(
            "Test User",
            "https://example.com/confirm");

        // Act
        var result = await renderer.RenderAsync("ConfirmEmail", model);


        // Assert
        Assert.Contains($"Hello {model.RecipientName}", result);
        Assert.Contains(model.ConfirmationUrl, result);
        Assert.Contains("Confirm your email", result);
    }
}
